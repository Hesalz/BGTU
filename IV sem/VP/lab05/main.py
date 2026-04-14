import cv2
import tkinter as tk
from tkinter import filedialog, ttk
from PIL import Image, ImageTk

class ImageEditor:
    def __init__(self, root):
        self.root = root
        self.root.title("Image Editor")
        self.root.geometry("700x650")
        self.root.configure(bg="#f0f0f0")

        self.image = None
        self.processed_image = None

        self.canvas = tk.Canvas(root, width=500, height=350, bg="white", relief=tk.SUNKEN, borderwidth=2)
        self.canvas.pack(pady=10)

        self.button_frame = tk.Frame(root, bg="#f0f0f0")
        self.button_frame.pack(pady=10)

        self.open_button = ttk.Button(self.button_frame, text="Open", command=self.open_image)
        self.open_button.grid(row=0, column=0, padx=5, pady=5)

        self.reset_button = ttk.Button(self.button_frame, text="Reset", command=self.reset_image)
        self.reset_button.grid(row=0, column=1, padx=5, pady=5)

        self.save_button = ttk.Button(self.button_frame, text="Save", command=self.save_image)
        self.save_button.grid(row=0, column=2, padx=5, pady=5)

        self.edit_frame = tk.LabelFrame(root, text="Edit Tools", bg="#f0f0f0", padx=10, pady=10)
        self.edit_frame.pack(pady=10)

        self.rotate_button = ttk.Button(self.edit_frame, text="Rotate 90°", command=self.rotate_image)
        self.rotate_button.grid(row=0, column=0, padx=5, pady=5)

        self.gray_button = ttk.Button(self.edit_frame, text="Grayscale", command=self.convert_to_gray)
        self.gray_button.grid(row=0, column=1, padx=5, pady=5)

        self.light_button = ttk.Button(self.edit_frame, text="Brighten", command=self.adjust_brightness)
        self.light_button.grid(row=1, column=0, padx=5, pady=5)

        self.blur_button = ttk.Button(self.edit_frame, text="Blur", command=self.apply_blur)
        self.blur_button.grid(row=1, column=1, padx=5, pady=5)

        self.contrast_slider = ttk.Scale(self.edit_frame, from_=0.5, to=3.0, orient=tk.HORIZONTAL, command=self.adjust_contrast)
        self.contrast_slider.grid(row=2, column=0, columnspan=2, pady=10)
        self.contrast_label = ttk.Label(self.edit_frame, text="Contrast")
        self.contrast_label.grid(row=3, column=0, columnspan=2)

    def open_image(self):
        file_path = filedialog.askopenfilename()
        if file_path:
            self.image = cv2.imread(file_path)
            self.processed_image = self.image.copy()
            self.display_image()
    
    def display_image(self):
        img = cv2.cvtColor(self.processed_image, cv2.COLOR_BGR2RGB)
        img = Image.fromarray(img)
        img.thumbnail((500, 350))
        self.tk_image = ImageTk.PhotoImage(img)
        self.canvas.create_image(250, 175, image=self.tk_image, anchor=tk.CENTER)
    
    def reset_image(self):
        if self.image is not None:
            self.processed_image = self.image.copy()
            self.display_image()
    
    def save_image(self):
        if self.processed_image is not None:
            file_path = filedialog.asksaveasfilename(defaultextension=".png", filetypes=[("PNG files", "*.png"), ("JPEG files", "*.jpg")])
            if file_path:
                cv2.imwrite(file_path, self.processed_image)
    
    def rotate_image(self):
        if self.processed_image is not None:
            self.processed_image = cv2.rotate(self.processed_image, cv2.ROTATE_90_CLOCKWISE)
            self.display_image()
    
    def convert_to_gray(self):
        if self.processed_image is not None:
            self.processed_image = cv2.cvtColor(self.processed_image, cv2.COLOR_BGR2GRAY)
            self.processed_image = cv2.cvtColor(self.processed_image, cv2.COLOR_GRAY2BGR)
            self.display_image()
    
    def adjust_brightness(self):
        if self.processed_image is not None:
            self.processed_image = cv2.convertScaleAbs(self.processed_image, alpha=1.2, beta=30)
            self.display_image()
    
    def apply_blur(self):
        if self.processed_image is not None:
            self.processed_image = cv2.GaussianBlur(self.processed_image, (5, 5), 0)
            self.display_image()
    
    def adjust_contrast(self, event=None):
        if self.processed_image is not None:
            alpha = self.contrast_slider.get()
            self.processed_image = cv2.convertScaleAbs(self.image, alpha=alpha, beta=0)
            self.display_image()

if __name__ == "__main__":
    root = tk.Tk()
    app = ImageEditor(root)
    root.mainloop()
