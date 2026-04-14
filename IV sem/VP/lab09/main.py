import matplotlib.pyplot as plt
from wordcloud import WordCloud, STOPWORDS
from PIL import Image
import numpy as np
import string
import pymorphy3 as pymorphy2
from collections import Counter
import os

def load_text(filename):
    with open(filename, 'r', encoding='utf-8') as file:
        text = file.read()
    return text

def process_text(text, extra_stopwords=None):
    morph = pymorphy2.MorphAnalyzer()
    text = text.translate(str.maketrans('', '', string.punctuation))
    words = text.lower().split()
    stopwords = set(STOPWORDS)
    
    if extra_stopwords:
        stopwords.update(extra_stopwords)
    
    processed_words = []
    for word in words:
        if word not in stopwords and len(word) > 2:
            parsed_word = morph.parse(word)[0]
            normal_form = parsed_word.normal_form
            processed_words.append(normal_form)
    
    return ' '.join(processed_words)

def create_wordcloud(text, mask_image=None, output_file='wordcloud.png'):
    if mask_image and os.path.exists(mask_image):
        try:
            mask = Image.open(mask_image)
            if mask.mode == 'RGBA':
                background = Image.new('RGB', mask.size, (255, 255, 255))
                background.paste(mask, mask=mask.split()[3])
                mask = background
        
            mask = np.array(mask.convert('L'))
            mask = np.where(mask > 128, 255, 0).astype(np.uint8)
            
        except Exception as e:
            print(f"Ошибка обработки маски: {e}")
            mask = None
    else:
        mask = None
    
    wordcloud = WordCloud(
        width=800,
        height=800,
        background_color='white',
        mask=mask,
        contour_width=0, 
        colormap='viridis',
        max_words=200,
        prefer_horizontal=0.9,
        relative_scaling=0.5
    ).generate(text)

    plt.figure(figsize=(10, 10), facecolor='white', edgecolor='white')
    plt.imshow(wordcloud, interpolation='bilinear')
    plt.axis('off')
    plt.tight_layout(pad=0)
    plt.savefig(output_file, bbox_inches='tight', pad_inches=0, dpi=300, transparent=False)
    plt.show()

def main():
    input_file = 'text.txt'
    mask_image = 'mask.jpg'
    output_file = 'wordcloud_output.png'
    extra_stopwords = ['который', 'весь', 'это', 'наш']
    
    if not os.path.exists(input_file):
        print(f"Файл {input_file} не найден!")
        return
    
    raw_text = load_text(input_file)
    print(f"Загружено {len(raw_text.split())} слов из файла.")
    
    processed_text = process_text(raw_text, extra_stopwords)
    print(f"После обработки осталось {len(processed_text.split())} значимых слов.")
    
    if mask_image and os.path.exists(mask_image):
        print(f"Используется маска из файла {mask_image}")
        wordcloud = create_wordcloud(processed_text, mask_image, output_file)
    else:
        print("Маска не найдена, создается стандартное облако слов.")
        wordcloud = create_wordcloud(processed_text, None, output_file)
    
    print(f"Облако слов сохранено в файл {output_file}")

if __name__ == '__main__':
    main()