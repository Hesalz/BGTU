int LEDrot = 12;
int LEDgelb = 11;
int LEDgruen = 10;
int cnt = 0;
int state = 1;
void setup()
{
	pinMode(LEDrot, OUTPUT);
	pinMode(LEDgelb, OUTPUT);
	pinMode(LEDgruen, OUTPUT);
}

// the loop function runs over and over again until power down or reset
void loop() {
	cnt++;
	if (cnt == 100)
	{
		cnt = 0;
		Statemaschine();
	}
	delay(10);
}

void Statemaschine(void)
{

	switch (state)
	{
	case 1:
		digitalWrite(LEDrot, HIGH);
		digitalWrite(LEDgelb, LOW);
		digitalWrite(LEDgruen, LOW);
		state++;
		break;

	case 2:
		digitalWrite(LEDrot, HIGH);
		digitalWrite(LEDgelb, HIGH);
		digitalWrite(LEDgruen, LOW);
		state++;
		break;
	case 3:
		digitalWrite(LEDrot, LOW);
		digitalWrite(LEDgelb, LOW);
		digitalWrite(LEDgruen, HIGH);
		state++;
		break;

	case 4:
		digitalWrite(LEDrot, LOW);
		digitalWrite(LEDgelb, HIGH);
		digitalWrite(LEDgruen, LOW);
		state = 1;
		break;
	}
}
