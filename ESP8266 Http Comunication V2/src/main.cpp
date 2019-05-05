/*
  During creation of this file there were used following websites as source of knowledge:
  https://randomnerdtutorials.com/decoding-and-encoding-json-with-arduino-or-esp8266/
  https://arduinojson.org/v5/assistant/

  ArduinoJson version: 5.13.2
*/

#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266HTTPClient.h>
#include <ArduinoJson.h> //ver. 5.13.2
//
//---------------------------------------------------------------------

const char *wifiName = "Tararituta";
const char *wifiPass = "qwertyasd";
const char *JWTtoken;
bool getMessagesTrigger, postMessageTrigger, logInTrigger, updateDisplayTrigger, readGestureTrigger;
long long getMessagesTimer, getMessagesTimePeriod;
HTTPClient http;
String json;
class Message
{
public:
	const char *MessageContent;
	const char *MessageAuthor;
	const char *MessageException; //To receive - errors inside REST
	const char *MessageDeath;     //Format: dd.MM.yyyy HH:mm:ss - for example 17.04.2019 19:49:30
	const char *MessagePriority;  //Available: "HIGH", "MEDIUM", "LOW" - every other option will be assigned as "MEDIUM"
	const char *MessageInfoMsg;   //To receive - errors with processing and comunication
};
Message messages[5];

//---------------------------------------------------------------------

//This function downloads five sorted messages from REST api
//Messages are firstly ordered by their priority and secondly by expiry date
//All downloaded data is saved in global messages array
void GetMessages()
{
	for (int i = 0; i < 5; i++)
	{
		messages[i].MessageAuthor = "";
		messages[i].MessageContent = "";
		messages[i].MessageException = "";
		messages[i].MessageDeath = "";
		messages[i].MessageInfoMsg = "";
		messages[i].MessagePriority = "";
	}
	const char *host = "http://postiotapi.ipprograms.pl/api/values/get-best-five-messages";
	http.begin(host);
	http.addHeader("Authorization", "Bearer " + String(JWTtoken));
	int httpResponseCode = http.GET();
	json = http.getString();
	if (httpResponseCode == 200)
	{
		const size_t JSONcapacity = JSON_ARRAY_SIZE(5) + JSON_OBJECT_SIZE(8) + 2000;
		DynamicJsonBuffer jsonBufferSpecific(JSONcapacity);

		JsonArray &parser = jsonBufferSpecific.parseArray(json);
		if (!parser.success())
		{
			Serial.println(F("Błąd przetwarzania pliku JSON!"));
			messages[0].MessageInfoMsg = "Błąd przetwarzania pliku JSON!";
			http.end();
			return;
		}

		for (int i = 0; i < 5; i++)
		{
			messages[i].MessageAuthor = parser[i]["MessageAuthor"].as<char *>();
			Serial.println(messages[i].MessageAuthor);
			messages[i].MessageContent = parser[i]["MessageContent"].as<char *>();
			Serial.println(messages[i].MessageContent);
			messages[i].MessageException = parser[i]["MessageException"].as<char *>();
			Serial.println(messages[i].MessageException);
			messages[i].MessageInfoMsg = parser[i]["InfoMsg"].as<char *>();
			Serial.println(messages[i].MessageInfoMsg);
			Serial.println();
		}
		http.end();
	}
	else
	{
		Serial.println(F("Nie udało się uzyskać odpowiedzi serwera"));
		messages[0].MessageInfoMsg = "Nie udało się uzyskać odpowiedzi serwera";
		http.end();
		return;
	}
}

//This function adds single message to database by REST api
//All variables should be set in messages[0] object before calling PostMessage function
//Remeber about setting one of three possible MessagePriority: HIGH, MEDIUM, LOW. Otherwise it will be set on MEDIUM by default
//There are two possible options for JSON format
//If you want to set a specific expiry date for message you need to write it into MessageDeath with format dd.MM.yyyy HH:mm:ss
//Wrong format will return REST api processing error so make sure you have written it correctly
//If you don't want to have expiry date for message you need to set MessageDeath on empty
void PostMessage()
{
	//Operation required for easy and comfortable working with char content
	String messageContent = messages[0].MessageContent;
	String messageAuthor = messages[0].MessageAuthor;
	String messagePriority = messages[0].MessagePriority;
	String messageDeath = messages[0].MessageDeath;
	if (messageDeath != "")
	{
		json = "{\"MessageContent\": \"" + messageContent + "\", \"MessageAuthor\": \"" + messageAuthor + "\", \"MessagePriority\": \"" + messagePriority + "\", \"MessageDeath\": \"" + messageDeath + "\"}";
	}
	else
	{
		json = "{\"MessageContent\": \"" + messageContent + "\", \"MessageAuthor\": \"" + messageAuthor + "\", \"MessagePriority\": \"" + messagePriority + "\"}";
	}
	const char *host = "http://postiotapi.ipprograms.pl/api/values/new-message";
	http.begin(host);
	http.addHeader("Host", "postiotapi.ipprograms.pl");
	http.addHeader("Content-Type", "application/json");
	http.addHeader("Authorization", "Bearer " + String(JWTtoken));
	http.addHeader("Content-Length", String(json.length()));
	int httpResponseCode = http.POST(json);
	String receivedJson = http.getString();
	if (httpResponseCode == 200)
	{
		const size_t JSONcapacity = JSON_ARRAY_SIZE(1) + JSON_OBJECT_SIZE(1) + 200;
		DynamicJsonBuffer jsonBufferSpecific(JSONcapacity);

		JsonArray &parser = jsonBufferSpecific.parseArray(receivedJson);
		if (!parser.success())
		{
			Serial.println(F("Błąd przetwarzania pliku JSON!"));
			messages[0].MessageInfoMsg = "Błąd przetwarzania pliku JSON!";
			http.end();
			return;
		}
		messages[0].MessageInfoMsg = parser[0]["InfoMsg"].as<char *>();
		Serial.println(messages[0].MessageInfoMsg);
		http.end();
	}
	else
	{
		Serial.println(F("Nie udało się uzyskać odpowiedzi serwera"));
		messages[0].MessageInfoMsg = "Nie udało się uzyskać odpowiedzi serwera";
		http.end();
		return;
	}
}

//This function sends POST request with login and password to receive JSON Web Token in JSON file
//Token is written to global function and needs to be sent with every request to REST api
//Token is valid for 2 hours
void LogIn()
{
	json = "{\"UserLogin\": \"ESP8266_post_iot\", \"UserPassword\": \"c5d5ea201ebe9561f5ba93312264030e27ec777a745e55c7225262bf0f2ab9dd\"}";
	const char *host = "http://postiotapi.ipprograms.pl/api/login";
	http.begin(host);
	http.addHeader("Host", "postiotapi.ipprograms.pl");
	http.addHeader("Content-Type", "application/json");
	http.addHeader("Content-Length", String(json.length()));
	int httpResponseCode = http.POST(json);
	String receivedJson = http.getString();
	if (httpResponseCode == 200)
	{
		const size_t JSONcapacity = JSON_OBJECT_SIZE(1) + 200;
		DynamicJsonBuffer jsonBufferSpecific(JSONcapacity);

		JsonObject &parser = jsonBufferSpecific.parseObject(receivedJson);
		if (!parser.success())
		{
			Serial.println(F("Błąd przetwarzania pliku JSON!"));
			JWTtoken = "Błąd przetwarzania pliku JSON!";
			http.end();
			return;
		}
		JWTtoken = parser["token"].as<char *>();
		http.end();
	}
	else
	{
		Serial.println(F("Nie udało się uzyskać odpowiedzi serwera"));
		JWTtoken = "Nie udało się uzyskać odpowiedzi serwera";
		http.end();
		return;
	}
}

void setup()
{
	Serial.begin(115200);
	WiFi.begin(wifiName, wifiPass);
	while (WiFi.status() != WL_CONNECTED)
	{
		delay(500);
		Serial.println(F("Connecting... "));
	}
	getMessagesTrigger = 0;
	postMessageTrigger = 0;
	logInTrigger = 1;
	updateDisplayTrigger = 0;
	readGestureTrigger = 0;
	getMessagesTimePeriod = 60000;
}

void loop()
{
	if (logInTrigger == 1)
	{
		LogIn();
		getMessagesTimer = millis() - getMessagesTimePeriod;
		logInTrigger = 0;
		getMessagesTrigger = 1;
	}
	if (getMessagesTrigger == 1)
	{
		if (millis() - getMessagesTimer > getMessagesTimePeriod)  //It may causse problems after 49 days when timer will rollover and return to 0. Look at: https://www.baldengineer.com/arduino-how-do-you-reset-millis.html
		{
			GetMessages();
			getMessagesTimer = millis(); 
			updateDisplayTrigger = 1;
		}
	}
	if (postMessageTrigger == 1)
	{
		//Operation required for easy and comfortable working with char content
		String messageContent = messages[0].MessageContent;
		String messageAuthor = messages[0].MessageAuthor;

		if (messageContent != "" && messageAuthor != "")
		{
			//Example content
			messages[0].MessageContent = "Łaazaaaaaaaaaaaaa";
			messages[0].MessageAuthor = "ScaryMan";
			messages[0].MessagePriority = "HIGH";
			messages[0].MessageDeath = "29.04.2019 19:49:30";

			PostMessage();
			messages[0].MessageContent = "";
			messages[0].MessageAuthor = "";
			messages[0].MessagePriority = "";
			messages[0].MessageDeath = "";
			messages[0].MessageException = "";
			messages[0].MessageInfoMsg = "";
			postMessageTrigger = 0;
		}
		if (readGestureTrigger == 1)
		{
			//Code for gesture reading
			updateDisplayTrigger = 1;
		}
		if (updateDisplayTrigger == 1)
		{
			//Code for display update
		}
	}
}