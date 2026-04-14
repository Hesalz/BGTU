// Подключение необходимых библиотек
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>
#include <Wire.h>
#include <SPI.h>

// Константы экрана
#define SCREEN_WIDTH 128
#define SCREEN_HEIGHT 64
#define OLED_RESET -1
Adafruit_SSD1306 display(SCREEN_WIDTH, SCREEN_HEIGHT, &Wire, OLED_RESET);

// Пины для кнопок
#define BUTTON_LEFT 2
#define BUTTON_RIGHT 3
#define BUTTON_START 4
#define FLAP_BUTTON 4

// Параметры игры Flappy Bird
#define SPRITE_HEIGHT 16
#define SPRITE_WIDTH 16
#define GAME_SPEED 50

// Данные для Flappy Bird
static const unsigned char PROGMEM wing_down_bmp[] =
{ B00000000, B00000000,
  B00000000, B00000000,
  B00000011, B11000000,
  B00011111, B11110000,
  B00111111, B00111000,
  B01111111, B11111110,
  B11111111, B11000001,
  B11011111, B01111110,
  B11011111, B01111000,
  B11011111, B01111000,
  B11001110, B01111000,
  B11110001, B11110000,
  B01111111, B11100000,
  B00111111, B11000000,
  B00000111, B00000000,
  B00000000, B00000000,
};

static const unsigned char PROGMEM wing_up_bmp[] =
{ B00000000, B00000000,
  B00000000, B00000000,
  B00000011, B11000000,
  B00011111, B11110000,
  B00111111, B00111000,
  B01110001, B11111110,
  B11101110, B11000001,
  B11011111, B01111110,
  B11011111, B01111000,
  B11111111, B11111000,
  B11111111, B11111000,
  B11111111, B11110000,
  B01111111, B11100000,
  B00111111, B11000000,
  B00000111, B00000000,
  B00000000, B00000000,
};

// Переменные игры Flappy Bird
int game_state = 1; // 0 = игра, 1 = конец игры
int score = 0;
int high_score = 0;
int bird_x = SCREEN_WIDTH / 4;
int bird_y;
int momentum = 0;
int wall_x[2];
int wall_y[2];
int wall_gap = 30;
int wall_width = 10;

// Переменные для меню
int currentGame = 0; // 0 = Flappy Bird, 1 = Snake, 2 = PingPong
bool gameRunning = false;

// Прототипы функций
void drawMenu();
void playFlappy();
void playSnake();
void playPingPong();
void show_start_screen_flappy();
void show_game_over_screen_flappy();

// Snake Variables
#define RIGHT 0
#define LEFT  1
#define UP    2
#define DOWN  3

#define BUZZER 12
#define UP_BTN    4
#define DOWN_BTN  3
#define LEFT_BTN  5
#define RIGHT_BTN 2

const uint8_t block[] PROGMEM = {
  0xf0, // B11110000
  0xb0, // B10110000
  0xd0, // B11010000
  0xf0, // B11110000
};

uint8_t snake_head_x = SCREEN_WIDTH / 2; // Center the snake horizontally
uint8_t snake_head_y = SCREEN_HEIGHT / 2; // Center the snake vertically
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
int score_snake = 0;
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
  display.drawBitmap(x, y, block, 4, 4, 1);
}

void show_score(int x, int y, int data) {
  display.setCursor(x, y);
  display.println(data);
}

void screen() {
  display.clearDisplay();
  display.setTextSize(1);
  display.drawRect(0, 1, 102, 62, 1);
  display.drawRect(0, 0, 102, 64, 1);
  display.setCursor(104, 12);
  display.println("lvl");
  display.setCursor(104, 40);
  display.println("scr");

  show_score(110, 25, level);
  show_score(110, 53, score_snake);

  for (i = 0; i < snake_len; i++) {
    draw_snake(x[i], y[i]);
  }

  draw_snake(food_x, food_y);

  display.display();
}

void draw_food() {
  int food_out = 0;

  if (food_eaten) {
    while (food_out == 0) {
      food_out = 1;

      food_x = (uint8_t)(random(4, 100) / 4) * 4;
      food_y = (uint8_t)(random(8, 60) / 4) * 4; // Ensure food does not spawn at the top

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
    score_snake++;
    level = score_snake / 5 + 1;
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
  display.clearDisplay();
  display.setTextSize(2);
  display.setCursor(10, 10);

  display.println("GAME OVER");
  display.setTextSize(1);
  display.setCursor(30, 35);
  display.println("LEVEL:");
  display.setCursor(30, 55);
  display.println("SCORE:");

  show_score(80, 35, level);
  show_score(80, 55, score_snake);

  display.display();
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
  display.clearDisplay();
  display.setTextSize(2);
  display.setCursor(35, 10);
  display.println("SNAKE");
  display.setTextSize(1);
  display.setCursor(15, 40);
  display.println("Press any button");
  display.display();

  // Wait for any button press
  while (digitalRead(UP_BTN) == HIGH && digitalRead(DOWN_BTN) == HIGH &&
         digitalRead(LEFT_BTN) == HIGH && digitalRead(RIGHT_BTN) == HIGH) {
    delay(10);
  }
}

void handle_game_over() {
  draw_game_over();
  delay(1000);
  while (digitalRead(UP_BTN) == HIGH && digitalRead(DOWN_BTN) == HIGH &&
         digitalRead(LEFT_BTN) == HIGH && digitalRead(RIGHT_BTN) == HIGH) {
    delay(10);
  }
  reset_game();
}

void reset_game() {
  snake_head_x = SCREEN_WIDTH / 2; // Center the snake horizontally
  snake_head_y = SCREEN_HEIGHT / 2; // Center the snake vertically
  snake_len = 2;
  snake_dir = RIGHT;
  food_eaten = true;
  game_over = false;
  score_snake = 0;
  level = 1;
  snake_speed = 150;
}

// PingPong Variables
#define UP_BUTTON 2
#define DOWN_BUTTON 3
#define UP_BUTTON_2 4
#define DOWN_BUTTON_2 5

const unsigned long PADDLE_RATE = 33;
const unsigned long BALL_RATE = 16;
const uint8_t PADDLE_HEIGHT = 24;
float ball_speed_multiplier = 1.0;
const float speed_increment = 0.1; // Увеличение скорости после каждого удара
const float max_speed_multiplier = 2.5; // Максимальное увеличение скорости

// Paddle and ball variables
uint8_t ball_x = 64, ball_y = 32;
int8_t ball_dir_x = 1, ball_dir_y = 1;
unsigned long ball_update;

unsigned long paddle_update;
const uint8_t CPU_X = 12;
uint8_t cpu_y = 16;
uint8_t new_x = ball_x + ball_dir_x * ball_speed_multiplier;
uint8_t new_y = ball_y + ball_dir_y * ball_speed_multiplier;

const uint8_t PLAYER_X = 115;
uint8_t player_y = 16;

bool game_over_pingpong = false;

void drawCourt();
void resetGamePingPong();
void updateBall();
void updatePaddles();
void handleInput(bool &up_state, bool &down_state);
void show_start_screen_pingpong();  // Start screen function
bool checkPaddleCollision(uint8_t new_x, uint8_t new_y, uint8_t paddle_x, uint8_t paddle_y);
void increaseBallSpeed();

void setup() {
    // Настройка кнопок
    pinMode(BUTTON_LEFT, INPUT_PULLUP);
    pinMode(BUTTON_RIGHT, INPUT_PULLUP);
    pinMode(BUTTON_START, INPUT_PULLUP);
    pinMode(FLAP_BUTTON, INPUT_PULLUP);

    // Инициализация дисплея
    if (!display.begin(SSD1306_SWITCHCAPVCC, 0x3C)) {
        for (;;);
    }
    display.clearDisplay();
    drawMenu();

    // Snake game setup
    display.begin(SSD1306_SWITCHCAPVCC, 0x3C);
    display.setTextColor(WHITE);
    randomSeed(analogRead(3));

    pinMode(BUZZER, OUTPUT);
    pinMode(UP_BTN, INPUT_PULLUP);
    pinMode(DOWN_BTN, INPUT_PULLUP);
    pinMode(LEFT_BTN, INPUT_PULLUP);
    pinMode(RIGHT_BTN, INPUT_PULLUP);

    // PingPong game setup
    pinMode(UP_BUTTON, INPUT_PULLUP);
    pinMode(DOWN_BUTTON, INPUT_PULLUP);
    pinMode(UP_BUTTON_2, INPUT_PULLUP);
    pinMode(DOWN_BUTTON_2, INPUT_PULLUP);
}

void loop() {
    if (!gameRunning) {
        // Переключение игр в меню
        if (digitalRead(BUTTON_START) == LOW) {
            currentGame = (currentGame - 1 + 3) % 3;
            delay(200);
            drawMenu();
        }
        if (digitalRead(BUTTON_RIGHT) == LOW) {
            currentGame = (currentGame + 1) % 3;
            delay(200);
            drawMenu();
        }
        if (digitalRead(BUTTON_LEFT) == LOW) {
            delay(200);
            gameRunning = true;
            switch (currentGame) {
                case 0: playFlappy(); break;
                case 1: playSnake(); break;
                case 2: playPingPong(); break;
            }
            gameRunning = false;
            drawMenu();
        }
    }
}

void drawMenu() {
    display.clearDisplay();
    display.setTextSize(1);
    display.setTextColor(SSD1306_WHITE);
    display.setCursor(0, 0);
    display.print("Select Game:");

    display.setCursor(0, 16);
    display.print(currentGame == 0 ? "> Flappy Bird" : "  Flappy Bird");

    display.setCursor(0, 32);
    display.print(currentGame == 1 ? "> Snake" : "  Snake");

    display.setCursor(0, 48);
    display.print(currentGame == 2 ? "> PingPong" : "  PingPong");

    display.display();
}

void playFlappy() {
    show_start_screen_flappy();

    bird_y = SCREEN_HEIGHT / 2;
    momentum = -4;
    wall_x[0] = SCREEN_WIDTH;
    wall_y[0] = SCREEN_HEIGHT / 2 - wall_gap / 2;
    wall_x[1] = SCREEN_WIDTH + SCREEN_WIDTH / 2;
    wall_y[1] = SCREEN_HEIGHT / 2 - wall_gap / 1;
    score = 0;
    game_state = 0;

    while (true) {
        if (game_state == 0) {
            display.clearDisplay();

            if (digitalRead(FLAP_BUTTON) == LOW) {
                momentum = -4;
            }

            momentum += 1;
            bird_y += momentum;

            if (bird_y < 0) {
                bird_y = 0;
            }

            if (bird_y > SCREEN_HEIGHT - SPRITE_HEIGHT) {
                bird_y = SCREEN_HEIGHT - SPRITE_HEIGHT;
                momentum = -2;
            }

            if (momentum < 0) {
                if (random(2) == 0) {
                    display.drawBitmap(bird_x, bird_y, wing_down_bmp, SPRITE_WIDTH, SPRITE_HEIGHT, WHITE);
                } else {
                    display.drawBitmap(bird_x, bird_y, wing_up_bmp, SPRITE_WIDTH, SPRITE_HEIGHT, WHITE);
                }
            } else {
                display.drawBitmap(bird_x, bird_y, wing_up_bmp, SPRITE_WIDTH, SPRITE_HEIGHT, WHITE);
            }

            for (int i = 0; i < 2; i++) {
                display.fillRect(wall_x[i], 0, wall_width, wall_y[i], WHITE);
                display.fillRect(wall_x[i], wall_y[i] + wall_gap, wall_width, SCREEN_HEIGHT - wall_y[i] + wall_gap, WHITE);

                if (wall_x[i] < 0) {
                    wall_y[i] = random(0, SCREEN_HEIGHT - wall_gap);
                    wall_x[i] = SCREEN_WIDTH;
                }

                if (wall_x[i] == bird_x) {
                    score++;
                    high_score = max(score, high_score);
                }

                if ((bird_x + SPRITE_WIDTH > wall_x[i] && bird_x < wall_x[i] + wall_width) &&
                    (bird_y < wall_y[i] || bird_y + SPRITE_HEIGHT > wall_y[i] + wall_gap)) {
                    display.display();
                    delay(500);
                    game_state = 1;
                }

                wall_x[i] -= 4;
            }
            display.setTextSize(1);
            display.setTextColor(WHITE);
            display.setCursor(SCREEN_WIDTH / 2 - 1, 0); // Центрируем текст по горизонтали
            display.print("Score: ");
            display.print(score);

            // Рисуем текст с небольшим смещением для создания эффекта жирного шрифта
            display.setCursor(SCREEN_WIDTH / 2, 0);
            display.print("Score: ");
            display.print(score);

            display.display();
            delay(GAME_SPEED);
        } else {
            show_game_over_screen_flappy();
            break;
        }
    }
}

void playSnake() {
    show_start_screen();
    while (!game_over) {
        keyScan();
        snake_move();
        draw_food();
        screen();
        delay(snake_speed);
    }
    handle_game_over();
}

void playPingPong() {
    display.clearDisplay();
    display.display();

    display.clearDisplay();
    show_start_screen_pingpong();  // Show the start screen

    // Wait for any button press to start the game
    while (digitalRead(UP_BUTTON) == HIGH && digitalRead(DOWN_BUTTON) == HIGH &&
           digitalRead(UP_BUTTON_2) == HIGH && digitalRead(DOWN_BUTTON_2) == HIGH) {
        // Do nothing, just wait for button press
    }
    display.clearDisplay();  // Clear the screen before starting the game
    resetGamePingPong();

    while (true) {
        if (game_over_pingpong) {
            display.clearDisplay();

            // "GAME OVER" в верхней части экрана
            display.setTextSize(2);
            display.setTextColor(WHITE);
            display.setCursor(10, 20);  // Позиция для надписи "GAME OVER"
            display.print("GAME OVER");

            // "Press any button" ниже
            display.setTextSize(1);
            display.setCursor(25, 40);  // Позиция для надписи "Press any button"
            display.print("try again ^_^");

            display.display();
            delay(1000);
            // Ждем, пока пользователь нажмет любую кнопку
            while (digitalRead(UP_BUTTON) == HIGH && digitalRead(DOWN_BUTTON) == HIGH &&
                   digitalRead(UP_BUTTON_2) == HIGH && digitalRead(DOWN_BUTTON_2) == HIGH) {
                // Просто ждем нажатия кнопки
            }
            return;  // Возвращаемся, чтобы выйти из loop, и не выполнять остальной код
        }

        bool update = false;
        unsigned long time = millis();

        static bool up_state = false;
        static bool down_state = false;
        handleInput(up_state, down_state);

        if (time > ball_update) {
            updateBall();
            ball_update += BALL_RATE;
            update = true;
        }

        if (time > paddle_update) {
            updatePaddles();
            paddle_update += PADDLE_RATE;
            update = true;
        }

        if (update) {
            display.display();
        }
    }
}

void updateBall() {
    display.drawPixel(ball_x, ball_y, BLACK);

    uint8_t new_x = ball_x + ball_dir_x * ball_speed_multiplier;
    uint8_t new_y = ball_y + ball_dir_y * ball_speed_multiplier;

    // Check collisions with walls
    if (new_y == 0 || new_y == SCREEN_HEIGHT - 1) {
        ball_dir_y = -ball_dir_y;
        new_y += ball_dir_y + ball_dir_y;
    }

    // Check collisions with paddles
    if (checkPaddleCollision(new_x, new_y, PLAYER_X, player_y)) {
        ball_dir_x = -ball_dir_x;
        new_x += ball_dir_x + ball_dir_x;
        increaseBallSpeed();  // Увеличиваем скорость при ударе по ракетке
    } else if (checkPaddleCollision(new_x, new_y, CPU_X, cpu_y)) {
        ball_dir_x = -ball_dir_x;
        new_x += ball_dir_x + ball_dir_x;
        increaseBallSpeed();  // Увеличиваем скорость при ударе по ракетке
    }

    // Check for scoring
    if (new_x == 0 || new_x == SCREEN_WIDTH - 1) {
        game_over_pingpong = true;
        return;
    }

    ball_x = new_x;
    ball_y = new_y;

    display.drawPixel(ball_x, ball_y, WHITE);
}

bool checkPaddleCollision(uint8_t new_x, uint8_t new_y, uint8_t paddle_x, uint8_t paddle_y) {
    // Проверяем, что мяч движется по оси X к платформе
    if ((new_x >= paddle_x) && (new_x <= paddle_x + 1)) {
        // Проверяем, пересекает ли мяч платформу по оси Y
        if ((new_y >= paddle_y) && (new_y <= paddle_y + PADDLE_HEIGHT)) {
            return true;
        }
    }
    return false;
}

void increaseBallSpeed() {
    // Увеличиваем скорость мяча после удара, но не больше максимального множителя
    ball_speed_multiplier += speed_increment;
    if (ball_speed_multiplier > max_speed_multiplier) {
        ball_speed_multiplier = max_speed_multiplier;
    }
}

void resetGamePingPong() {
    ball_x = SCREEN_WIDTH / 2;
    ball_y = SCREEN_HEIGHT / 2;
    ball_dir_x = 1;
    ball_dir_y = 1;
    cpu_y = 16;
    player_y = 16;
    ball_update = millis();
    paddle_update = ball_update;
    game_over_pingpong = false;
    ball_speed_multiplier = 1.0;
    display.clearDisplay();
    drawCourt();
}

void drawCourt() {
    display.drawRect(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT, WHITE);
    display.display();
}

void show_start_screen_pingpong() {
    display.clearDisplay();

    // "PING PONG" в верхней части экрана
    display.setTextSize(2);
    display.setTextColor(WHITE);
    display.setCursor(10, 20);  // Позиция для надписи "PING PONG"
    display.print("PING PONG");

    // "Press any button" ниже
    display.setTextSize(1);
    display.setCursor(15, 40);  // Позиция для надписи "Press any button"
    display.print("Press any button");

    display.display();
}

void updatePaddles() {
    // Player 2 paddle (formerly CPU)
    display.drawFastVLine(CPU_X, cpu_y, PADDLE_HEIGHT, BLACK);
    if (digitalRead(UP_BUTTON_2) == LOW) {
        cpu_y--;
    }
    if (digitalRead(DOWN_BUTTON_2) == LOW) {
        cpu_y++;
    }
    if (cpu_y < 1) cpu_y = 1;
    if (cpu_y + PADDLE_HEIGHT > SCREEN_HEIGHT - 1) cpu_y = SCREEN_HEIGHT - 1 - PADDLE_HEIGHT;
    display.drawFastVLine(CPU_X, cpu_y, PADDLE_HEIGHT, WHITE);

    // Player 1 paddle
    display.drawFastVLine(PLAYER_X, player_y, PADDLE_HEIGHT, BLACK);
    if (digitalRead(UP_BUTTON) == LOW) {
        player_y--;
    }
    if (digitalRead(DOWN_BUTTON) == LOW) {
        player_y++;
    }
    if (player_y < 1) player_y = 1;
    if (player_y + PADDLE_HEIGHT > SCREEN_HEIGHT - 1) player_y = SCREEN_HEIGHT - 1 - PADDLE_HEIGHT;
    display.drawFastVLine(PLAYER_X, player_y, PADDLE_HEIGHT, WHITE);
}

void handleInput(bool &up_state, bool &down_state) {
    up_state |= (digitalRead(UP_BUTTON) == LOW);
    down_state |= (digitalRead(DOWN_BUTTON) == LOW);
}

void show_start_screen_flappy() {
    display.clearDisplay();

    // "FLAPPY BIRD" в верхней части экрана
    display.setTextSize(2);
    display.setTextColor(WHITE);
    display.setCursor(30, 10);  // Позиция для надписи "FLAPPY BIRD"
    display.print("FLAPPY");
    display.setCursor(42, 30);
    display.print("BIRD");


    // "Press any button" ниже
    display.setTextSize(1);
    display.setCursor(15, 52);  // Позиция для надписи "Press any button"
    display.print("Press any button");

    display.display();

    // Wait for any button press
    while (digitalRead(UP_BTN) == HIGH && digitalRead(DOWN_BTN) == HIGH &&
         digitalRead(LEFT_BTN) == HIGH && digitalRead(RIGHT_BTN) == HIGH) {
    delay(10);
  }
}

void show_game_over_screen_flappy() {
    display.clearDisplay();

    // "GAME OVER" в верхней части экрана
    display.setTextSize(2);
    display.setTextColor(WHITE);
    display.setCursor(10, 20);  // Позиция для надписи "GAME OVER"
    display.print("GAME OVER");

    // "Score" ниже
    display.setTextSize(1);
    display.setCursor(40, 40);  // Позиция для надписи "Score"
    display.print("SCORE: ");
    display.print(score);

    display.setCursor(15, 50);  // Позиция для надписи "Score"
    display.print("press any button");

    display.display();

    delay(1000);

    // Wait for any button press
    while (digitalRead(UP_BTN) == HIGH && digitalRead(DOWN_BTN) == HIGH &&
         digitalRead(LEFT_BTN) == HIGH && digitalRead(RIGHT_BTN) == HIGH) {
    delay(10);
  }

    // Reset game state
    game_state = 0;
    score = 0;
    bird_y = SCREEN_HEIGHT / 2;
    momentum = -4;
    wall_x[0] = SCREEN_WIDTH;
    wall_y[0] = SCREEN_HEIGHT / 2 - wall_gap / 2;
    wall_x[1] = SCREEN_WIDTH + SCREEN_WIDTH / 2;
    wall_y[1] = SCREEN_HEIGHT / 2 - wall_gap / 1;
}
