int sensorPin = A0;
int sensorValue = 0;
char inputArray[240];
String inputString = ""; 
boolean stringComplete = false;

void setup(void) {
   Serial.begin(115200);
   pinMode(sensorPin, INPUT); 
}

void loop() {
  
  if (stringComplete){
    // Cleans input and separates in spaces
    inputString.trim();
    inputString.toCharArray(inputArray,240);
    char* command = strtok(inputArray, " ");
    
    // Checks commands and changes status
    if (strcmp(command,"REQ")==0){
      
      sensorValue = analogRead(sensorPin);
   Serial.println(sensorValue);
      
    } 
    else
    {
      Serial.println("E");
    }
  
    // Cleans variables used to receive serial data
    inputString = "";
    stringComplete = false;
    memset(&inputArray[0], 0, sizeof(inputArray));
  }
  
}

// Waits for serial data, and it's called everytime new data comes
// Full commands always end in '\n'
void serialEvent() 
{
  while (Serial.available()) 
  {
    char inChar = (char)Serial.read(); 
    inputString += inChar;
    if (inChar == '\n') 
    {
      stringComplete = true;
    } 
  }
}
