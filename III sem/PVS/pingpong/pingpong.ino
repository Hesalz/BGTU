#include <SPI.h>
#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>

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
#define SCREEN_WIDTH 128 // OLED display width, in pixels
#define SCREEN_HEIGHT 64 // OLED display height, in pixels
#define OLED_RESET 4

Adafruit_SSD1306 display(SCREEN_WIDTH, SCREEN_HEIGHT, &Wire, OLED_RESET);

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

bool game_over = false;

void drawCourt();
void resetGame();
void updateBall();
void updatePaddles();
void handleInput(bool &up_state, bool &down_state);
void show_start_screen();  // Start screen function
bool checkPaddleCollision(uint8_t new_x, uint8_t new_y, uint8_t paddle_x, uint8_t paddle_y);
void increaseBallSpeed();

void setup() {
    display.begin(SSD1306_SWITCHCAPVCC, 0x3C);
    display.clearDisplay();
    show_start_screen();  // Show the start screen

    pinMode(UP_BUTTON, INPUT_PULLUP);
    pinMode(DOWN_BUTTON, INPUT_PULLUP);
    pinMode(UP_BUTTON_2, INPUT_PULLUP);
    pinMode(DOWN_BUTTON_2, INPUT_PULLUP);

    // Wait for any button press to start the game
    while (digitalRead(UP_BUTTON) == HIGH && digitalRead(DOWN_BUTTON) == HIGH &&
           digitalRead(UP_BUTTON_2) == HIGH && digitalRead(DOWN_BUTTON_2) == HIGH) {
        // Do nothing, just wait for button press
    }
    display.clearDisplay();  // Clear the screen before starting the game
    resetGame();
}

void loop() {
    if (game_over) {
        display.clearDisplay();
    
        // "GAME OVER" в верхней части экрана
        display.setTextSize(2);
        display.setTextColor(WHITE);
        display.setCursor(10, 20);  // Позиция для надписи "GAME OVER"
        display.print("GAME OVER");
        
        // "Press any button" ниже
        display.setTextSize(1);
        display.setCursor(15, 40);  // Позиция для надписи "Press any button"
        display.print("try again -____-");

        display.display();

        // Ждем, пока пользователь нажмет любую кнопку
        while (digitalRead(UP_BUTTON) == HIGH && digitalRead(DOWN_BUTTON) == HIGH &&
               digitalRead(UP_BUTTON_2) == HIGH && digitalRead(DOWN_BUTTON_2) == HIGH) {
            // Просто ждем нажатия кнопки
        }

        // После нажатия кнопки сбрасываем игру
        resetGame();
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
        game_over = true;
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

void resetGame() {
    ball_x = SCREEN_WIDTH / 2;
    ball_y = SCREEN_HEIGHT / 2;
    ball_dir_x = 1;
    ball_dir_y = 1;
    cpu_y = 16;
    player_y = 16;
    ball_update = millis();
    paddle_update = ball_update;
    game_over = false;
    ball_speed_multiplier = 1.0;
    display.clearDisplay();
    drawCourt();
}

void drawCourt() {
    display.drawRect(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT, WHITE);
    display.display();
}

void show_start_screen() {
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
