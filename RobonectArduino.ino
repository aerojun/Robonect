/*
Blink
Turns on an LED on for one second, then off for one second, repeatedly.

This example code is in the public domain.
*/

// Pin 13 has an LED connected on most Arduino boards.
// give it a name:
int message = 0; // This will hold one byte of the serial message
int led = 8; // This is the pin that the led is conected to
int LEDD = 0; // The value or brightness of the LED, can be 0-255


// the setup routine runs once when you press reset:
void setup() {
	// initialize the digital pin as an output.
	Serial.begin(9600);
	pinMode(led, OUTPUT);
}

// the loop routine runs over and over again forever:
void loop() {

	if (Serial.available() > 0) { // Check to see if there is a new message
		message = Serial.read(); // Put the serial input into the message

		if (message == 'A'){ // If a capitol A is received
			digitalWrite(led, HIGH);   // turn the LED on (HIGH is the voltage level)
		}
		if (message == 'a'){ // If a lowercase a is received
			digitalWrite(led, LOW);   // turn the LED on (HIGH is the voltage level)
		}
	}



}