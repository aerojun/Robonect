
int led = 13; // This is the pin that the led is conected to
int message;

// the setup routine runs once when you press reset:
void setup() {
	// initialize the digital pin as an output.
	Serial.begin(9600);
	pinMode(led, OUTPUT);
}

// the loop routine runs over and over again forever:
void loop() {

	while (Serial.available()==0) {             //Wait for user input
        
            message = Serial.parseInt();
            
            if (Serial.read() == '\n') {
                if ( message == 1 )
                    digitalWrite(led, HIGH);
                else
                    digitalWrite(led, LOW);
            }
        }
}
