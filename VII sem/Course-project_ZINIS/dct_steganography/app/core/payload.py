import hashlib
import struct

from cryptography.fernet import Fernet, InvalidToken
from cryptography.hazmat.primitives import hashes
from cryptography.hazmat.primitives.kdf.pbkdf2 import PBKDF2HMAC


MAGIC = b"DCTS"
VERSION = 2
SALT_SIZE = 16

# magic + version + message length + salt + integrity hash
HEADER_FORMAT = ">4sBI16s32s"
HEADER_SIZE = struct.calcsize(HEADER_FORMAT)


def encrypted_size_for_message_bytes(message_bytes: int) -> int:
    """Return the Fernet token size for a plaintext of the given byte length."""
    if message_bytes < 0:
        raise ValueError("Message size cannot be negative.")

    # Fernet uses AES-CBC with PKCS7 padding. The token contains:
    # version(1) + timestamp(8) + IV(16) + ciphertext + HMAC(32).
    ciphertext_size = ((message_bytes // 16) + 1) * 16
    raw_size = 57 + ciphertext_size
    return 4 * ((raw_size + 2) // 3)


def payload_size_for_message_bytes(message_bytes: int) -> int:
    """Return the exact payload size for a plaintext of the given byte length."""
    return HEADER_SIZE + encrypted_size_for_message_bytes(message_bytes)


def max_message_bytes_for_payload_capacity(capacity_bytes: int) -> int:
    """Return the largest UTF-8 byte length that fits into a payload capacity."""
    if capacity_bytes < HEADER_SIZE:
        return 0

    low, high = 0, max(0, capacity_bytes - HEADER_SIZE)
    best = 0

    while low <= high:
        middle = (low + high) // 2
        if payload_size_for_message_bytes(middle) <= capacity_bytes:
            best = middle
            low = middle + 1
        else:
            high = middle - 1

    return best


def _derive_key(password: str, salt: bytes) -> bytes:
    """Derive a Fernet key from a user password."""
    if not password:
        raise ValueError("Password must not be empty.")

    kdf = PBKDF2HMAC(
        algorithm=hashes.SHA256(),
        length=32,
        salt=salt,
        iterations=300_000,
    )
    return __import__("base64").urlsafe_b64encode(
        kdf.derive(password.encode("utf-8"))
    )


def build_payload(message: str, password: str) -> bytes:
    """Encrypt a message and build a self-describing payload."""
    if not password:
        raise ValueError("Password must not be empty.")

    message_bytes = message.encode("utf-8")
    salt = __import__("secrets").token_bytes(SALT_SIZE)
    key = _derive_key(password, salt)

    encrypted = Fernet(key).encrypt(message_bytes)
    integrity = hashlib.sha256(message_bytes).digest()

    header = struct.pack(
        HEADER_FORMAT,
        MAGIC,
        VERSION,
        len(encrypted),
        salt,
        integrity,
    )

    return header + encrypted


def parse_payload(payload: bytes, password: str) -> str:
    """Validate, decrypt and decode a payload."""
    if len(payload) < HEADER_SIZE:
        raise ValueError("The hidden data is incomplete.")

    magic, version, encrypted_length, salt, integrity = struct.unpack(
        HEADER_FORMAT,
        payload[:HEADER_SIZE],
    )

    if magic != MAGIC:
        raise ValueError("No compatible DCT message was found.")

    if version != VERSION:
        raise ValueError("Unsupported payload version.")

    start = HEADER_SIZE
    end = start + encrypted_length

    if end > len(payload):
        raise ValueError("The hidden message is incomplete.")

    encrypted = payload[start:end]

    try:
        key = _derive_key(password, salt)
        message_bytes = Fernet(key).decrypt(encrypted)
    except (InvalidToken, ValueError) as exc:
        raise ValueError(
            "Invalid password or corrupted hidden data."
        ) from exc

    if hashlib.sha256(message_bytes).digest() != integrity:
        raise ValueError("Hidden data integrity check failed.")

    try:
        return message_bytes.decode("utf-8")
    except UnicodeDecodeError as exc:
        raise ValueError("Hidden message is not valid UTF-8.") from exc


def bytes_to_bits(data: bytes) -> list[int]:
    """Convert bytes to a sequence of bits."""
    return [
        (byte >> shift) & 1
        for byte in data
        for shift in range(7, -1, -1)
    ]


def bits_to_bytes(bits: list[int]) -> bytes:
    """Convert a sequence of bits back to bytes."""
    if len(bits) % 8 != 0:
        raise ValueError("Bit sequence length must be divisible by 8.")

    result = bytearray()

    for index in range(0, len(bits), 8):
        value = 0
        for bit in bits[index:index + 8]:
            value = (value << 1) | bit
        result.append(value)

    return bytes(result)
