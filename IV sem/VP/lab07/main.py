import pygame
import random
import sys


pygame.init()
pygame.mixer.init()

WIDTH, HEIGHT = 800, 600
FPS = 60
WHITE = (255, 255, 255)
BLACK = (0, 0, 0)
RED = (255, 0, 0)
GREEN = (0, 255, 0)
BLUE = (0, 0, 255)

screen = pygame.display.set_mode((WIDTH, HEIGHT))
pygame.display.set_caption("2D GAME")
clock = pygame.time.Clock()

try:
    coin_sound = pygame.mixer.Sound("coin.wav")
    hit_sound = pygame.mixer.Sound("hit.wav")
except:
    coin_sound = pygame.mixer.Sound(buffer=bytearray(100))
    hit_sound = pygame.mixer.Sound(buffer=bytearray(100))
    print("Звуковые файлы не найдены.")

class Player(pygame.sprite.Sprite):
    def __init__(self):
        super().__init__()
        self.original_image = pygame.Surface((30, 30), pygame.SRCALPHA)
        pygame.draw.polygon(self.original_image, BLUE, [(15, 0), (0, 30), (30, 30)])
        self.image = self.original_image
        self.rect = self.original_image.get_rect()
        self.rect.center = (WIDTH // 2, HEIGHT // 2)
        self.speed = 5
        self.health = 100
        self.score = 0
        self.direction = 0
    
    def update(self):
        keys = pygame.key.get_pressed()
        moving = False
        
        if keys[pygame.K_LEFT] or keys[pygame.K_RIGHT] or keys[pygame.K_UP] or keys[pygame.K_DOWN]:
            moving = True
            
            if keys[pygame.K_LEFT] and self.rect.left > 0:
                self.rect.x -= self.speed
                self.direction = 90
            if keys[pygame.K_RIGHT] and self.rect.right < WIDTH:
                self.rect.x += self.speed
                self.direction = 270
            if keys[pygame.K_UP] and self.rect.top > 0:
                self.rect.y -= self.speed
                self.direction = 0
            if keys[pygame.K_DOWN] and self.rect.bottom < HEIGHT:
                self.rect.y += self.speed
                self.direction = 180
                
            if keys[pygame.K_LEFT] and keys[pygame.K_UP]:
                self.direction = 45
            elif keys[pygame.K_LEFT] and keys[pygame.K_DOWN]:
                self.direction = 135
            elif keys[pygame.K_RIGHT] and keys[pygame.K_UP]:
                self.direction = 315
            elif keys[pygame.K_RIGHT] and keys[pygame.K_DOWN]:
                self.direction = 225
        
        if moving:
            self.image = pygame.transform.rotate(self.original_image, self.direction)
            self.rect = self.image.get_rect(center=self.rect.center)
    
    def draw_health(self, surface):
        health_width = 100 * (self.health / 100)
        health_bar = pygame.Rect(10, 10, health_width, 20)
        pygame.draw.rect(surface, RED, health_bar)
        pygame.draw.rect(surface, WHITE, (10, 10, 100, 20), 2)

class Coin(pygame.sprite.Sprite):
    def __init__(self):
        super().__init__()
        self.image = pygame.Surface((15, 15))
        self.image.fill(GREEN)
        self.rect = self.image.get_rect()
        self.rect.x = random.randrange(WIDTH - self.rect.width)
        self.rect.y = random.randrange(HEIGHT - self.rect.height)

class Enemy(pygame.sprite.Sprite):
    def __init__(self):
        super().__init__()
        self.image = pygame.Surface((25, 25))
        self.image.fill(RED)
        self.rect = self.image.get_rect()
        self.rect.x = random.randrange(WIDTH - self.rect.width)
        self.rect.y = random.randrange(HEIGHT - self.rect.height)
        self.speed_x = random.randrange(-3, 3)
        self.speed_y = random.randrange(-3, 3)
        if self.speed_x == 0:
            self.speed_x = 1
        if self.speed_y == 0:
            self.speed_y = 1
    
    def update(self):
        self.rect.x += self.speed_x
        self.rect.y += self.speed_y
        
        if self.rect.left < 0 or self.rect.right > WIDTH:
            self.speed_x *= -1
        if self.rect.top < 0 or self.rect.bottom > HEIGHT:
            self.speed_y *= -1

all_sprites = pygame.sprite.Group()
coins = pygame.sprite.Group()
enemies = pygame.sprite.Group()

player = Player()
all_sprites.add(player)

for i in range(10):
    coin = Coin()
    all_sprites.add(coin)
    coins.add(coin)

for i in range(5):
    enemy = Enemy()
    all_sprites.add(enemy)
    enemies.add(enemy)

font = pygame.font.SysFont('Arial', 24)

running = True
while running:
    clock.tick(FPS)
    
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False
        elif event.type == pygame.KEYDOWN:
            if event.key == pygame.K_ESCAPE:
                running = False
    
    all_sprites.update()
    
    hits = pygame.sprite.spritecollide(player, coins, True)
    for hit in hits:
        player.score += 10
        coin_sound.play()
        coin = Coin()
        all_sprites.add(coin)
        coins.add(coin)
    
    hits = pygame.sprite.spritecollide(player, enemies, False)
    if hits:
        player.health -= 1
        hit_sound.play()
        if player.health <= 0:
            running = False
    
    screen.fill(BLACK)
    all_sprites.draw(screen)
    
    player.draw_health(screen)
    
    score_text = font.render(f"Счет: {player.score}", True, WHITE)
    screen.blit(score_text, (WIDTH - 150, 10))
    
    pygame.display.flip()

pygame.quit()
sys.exit()