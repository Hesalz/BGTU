import pytest
from app.core.payload import (
    HEADER_SIZE, bits_to_bytes, bytes_to_bits, build_payload,
    max_message_bytes_for_payload_capacity, parse_payload,
    payload_size_for_message_bytes, encrypted_size_for_message_bytes,
)


def test_payload_round_trip_without_password():
    message = 'Привет, DCT!'
    assert parse_payload(build_payload(message), None) == message


def test_payload_round_trip_with_password():
    message = 'secret'
    payload = build_payload(message, 'correct-password')
    assert parse_payload(payload, 'correct-password') == message


def test_parse_encrypted_payload_requires_password():
    with pytest.raises(ValueError, match='необходимо ввести пароль'):
        parse_payload(build_payload('secret', 'pw'))


def test_parse_payload_rejects_corrupted_integrity():
    payload = bytearray(build_payload('hello'))
    payload[HEADER_SIZE] ^= 1
    with pytest.raises(ValueError, match='целостности'):
        parse_payload(bytes(payload))


def test_bits_to_bytes_rejects_non_byte_aligned_input():
    with pytest.raises(ValueError, match='кратна 8'):
        bits_to_bytes([1, 0, 1])


def test_capacity_result_fits_but_next_byte_does_not():
    capacity = payload_size_for_message_bytes(12)
    result = max_message_bytes_for_payload_capacity(capacity)
    assert result >= 12
    assert payload_size_for_message_bytes(result) <= capacity
    assert payload_size_for_message_bytes(result + 1) > capacity


