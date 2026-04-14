#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>

Adafruit_SSD1306 oled(128, 64, &Wire, -1);

#define RIGHT 0
#define LEFT  1
#define UP    2
#define DOWN  3

#define BUZZER 12 
// Button Pins
#define UP_BTN    4
#define DOWN_BTN  3
#define LEFT_BTN  5
#define RIGHT_BTN 2

// Snake Variables
const uint8_t block[] PROGMEM = {
  0xf0, // B11110000
  0xb0, // B10110000
  0xd0, // B11010000
  0xf0, // B11110000
};

uint8_t snake_head_x = 4;
uint8_t snake_head_y = 4;
uint8_t x[100];
uint8_t y[100];
uint8_t snake_len = 2;
uint8_t snake_dir = RIGHT;

uint8_t food_x;
uint8_t food_y;
bool food_eaten = true;

int click[] = { 1047 };
int click_duration[] = { 100 };
const int MELODY_LENGTH = 100;
bool game_over = false;
int score = 0;
int level = 1;
int snake_speed = 150;
int i;

unsigned long lastDebounceTime[4] = {0, 0, 0, 0}; // Время последнего нажатия для каждой кнопки
const int debounceDelay = 50; // Задержка для устранения дребезга (в миллисекундах)

unsigned long lastKeyPressTime = 0; // Время последнего изменения направления
const int keyDelay = 150; // Задержка между изменениями направления в миллисекундах

void keyScan() {
  unsigned long currentTime = millis(); // Текущее время

  // Проверяем, прошло ли достаточно времени с последнего изменения направления
  if (currentTime - lastKeyPressTime < keyDelay) {
    return; // Пропускаем обработку, если задержка ещё не истекла
  }

  // Проверяем каждую кнопку с учётом антидребезга
  if (digitalRead(UP_BTN) == LOW && snake_dir != DOWN && (currentTime - lastDebounceTime[UP] > debounceDelay)) {
    tone(BUZZER, click[0], 1000 / click_duration[0]);
    delay(100);
    noTone(BUZZER);
    snake_dir = UP;
    lastKeyPressTime = currentTime; // Обновляем время изменения направления
    lastDebounceTime[UP] = currentTime;
  } else if (digitalRead(DOWN_BTN) == LOW && snake_dir != UP && (currentTime - lastDebounceTime[DOWN] > debounceDelay)) {
    tone(BUZZER, click[0], 1000 / click_duration[0]);
    delay(100);
    noTone(BUZZER);
    snake_dir = DOWN;
    lastKeyPressTime = currentTime;
    lastDebounceTime[DOWN] = currentTime;
  } else if (digitalRead(LEFT_BTN) == LOW && snake_dir != RIGHT && (currentTime - lastDebounceTime[LEFT] > debounceDelay)) {
    tone(BUZZER, click[0], 1000 / click_duration[0]);
    delay(100);
    noTone(BUZZER);
    snake_dir = LEFT;
    lastKeyPressTime = currentTime;
    lastDebounceTime[LEFT] = currentTime;
  } else if (digitalRead(RIGHT_BTN) == LOW && snake_dir != LEFT && (currentTime - lastDebounceTime[RIGHT] > debounceDelay)) {
    tone(BUZZER, click[0], 1000 / click_duration[0]);
    delay(100);
    noTone(BUZZER);
    snake_dir = RIGHT;
    lastKeyPressTime = currentTime;
    lastDebounceTime[RIGHT] = currentTime;
  }
}


void draw_snake(int x, int y) {
  oled.drawBitmap(x, y, block, 4, 4, 1);
}

void show_score(int x, int y, int data) {
  oled.setCursor(x, y);
  oled.println(data);
}

void screen() {
  oled.clearDisplay();
  oled.setTextSize(1);
  oled.drawRect(0, 1, 102, 62, 1);
  oled.drawRect(0, 0, 102, 64, 1);
  oled.setCursor(104, 12);
  oled.println("lvl");
  oled.setCursor(104, 40);
  oled.println("scr");

  show_score(110, 25, level);
  show_score(110, 53, score);

  for (i = 0; i < snake_len; i++) {
    draw_snake(x[i], y[i]);
  }

  draw_snake(food_x, food_y);

  oled.display();
}

void draw_food() {
  int food_out = 0;

  if (food_eaten) {
    while (food_out == 0) {
      food_out = 1;

      food_x = (uint8_t)(random(4, 100) / 4) * 4;
      food_y = (uint8_t)(random(4, 60) / 4) * 4;

      for (int i = snake_len - 1; i > 0; i--) {
        if (food_x == x[i] && food_y == y[i]) {
          food_out = 0;
        }
      }
    }
  }

  food_eaten = false;
}

void snake_move() {
  switch (snake_dir) {
    case RIGHT:
      snake_head_x += 4;
      break;
    case UP:
      snake_head_y -= 4;
      break;
    case LEFT:
      snake_head_x -= 4;
      break;
    case DOWN:
      snake_head_y += 4;
      break;
  }

  if ((snake_head_x == food_x) && (snake_head_y == food_y)) {
    food_eaten = true;
    snake_len++;
    score++;
    level = score / 5 + 1;
    snake_speed -= level;
  }

  for (i = snake_len - 1; i > 0; i--) {
    x[i] = x[i - 1];
    y[i] = y[i - 1];
  }
  x[0] = snake_head_x;
  y[0] = snake_head_y;

  check_snake_die();
}

void draw_game_over() {
  oled.clearDisplay();
  oled.setTextSize(2);
  oled.setCursor(10, 10);

  oled.println("GAME OVER");
  oled.setTextSize(1);
  oled.setCursor(30, 35);
  oled.println("LEVEL:");
  oled.setCursor(30, 55);
  oled.println("SCORE:");

  show_score(80, 35, level);
  show_score(80, 55, score);

  oled.display();
}

void check_snake_die() {
  if (snake_head_x < 4 || snake_head_x > 96 || snake_head_y < 1 || snake_head_y > 56) {
    game_over = true;
  }

  if (snake_len > 4) {
    for (int i = 1; i < snake_len; i++) {
      if (snake_head_x == x[i] && snake_head_y == y[i]) {
        game_over = true;
      }
    }
  }
}

void show_start_screen() {
  oled.clearDisplay();
  oled.setTextSize(2);
  oled.setCursor(35, 10);
  oled.println("SNAKE");
  oled.setTextSize(1);
  oled.setCursor(15, 40);
  oled.println("Press any button");
  oled.display();

  // Wait for any button press
  while (digitalRead(UP_BTN) == HIGH && digitalRead(DOWN_BTN) == HIGH &&
         digitalRead(LEFT_BTN) == HIGH && digitalRead(RIGHT_BTN) == HIGH) {
    delay(10);
  }
}

void handle_game_over() {
  draw_game_over();
  delay(5000); // Pause for 5 seconds
  reset_game();
  show_start_screen();
}

void reset_game() {
  snake_head_x = 4;
  snake_head_y = 4;
  snake_len = 2;
  snake_dir = RIGHT;
  food_eaten = true;
  game_over = false;
  score = 0;
  level = 1;
  snake_speed = 150;
}

void setup() {
  oled.begin(SSD1306_SWITCHCAPVCC, 0x3C);
  oled.setTextColor(WHITE);
  randomSeed(analogRead(3));
  
  pinMode(BUZZER, OUTPUT);
  pinMode(UP_BTN, INPUT_PULLUP);
  pinMode(DOWN_BTN, INPUT_PULLUP);
  pinMode(LEFT_BTN, INPUT_PULLUP);
  pinMode(RIGHT_BTN, INPUT_PULLUP);

  show_start_screen();
}

void loop() {
  if (game_over) {
    handle_game_over();
  } else {
    keyScan();
    snake_move();
    draw_food();
    screen();
  }
  delay(snake_speed);
}
