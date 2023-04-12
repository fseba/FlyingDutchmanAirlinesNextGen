# FlyingDutchmanAirlinesNextGen

Das Grundgerüst stammt aus dem Buch ["Code like a pro in C#"](https://www.manning.com/books/code-like-a-pro-in-c-sharp) von Jort Rodenburg.  
Man überarbeitet legacy Code der Flying Dutchman Arline, um deren Flüge über einen externen Service abruf- und buchbar zu machen.  
Ich habe so viel Code wie möglich selbst geschrieben und anschließend mit den Vorschlägen im Buch verglichen und ggf. angepasst.  
Nachdem das Buch durch war, habe ich weitere Endpoints zum Abrufen und Löschen der Bookings ergänzt. 

Es ist aufgefallen, dass sehr viel mit Exceptions gearbeitet wurde.  
Die Anzahl an geworfenen Exceptions habe ich reduziert und zum Teil durch null returns oder andersweitig ersetzt.  
Dadurch wird der Code für andere einfacher handlebar und zwingt nicht gleich zu try/catch Blöcken.

Unit tests sind ein großes Thema im Buch und begleiten von Anfang an.  
Hier konnte ich einiges lernen und muss auch noch einiges lernen :grin: (Die nächsten Bücher sind schon in der Pipeline)  
Würde meine Tests eher als wild bezeichnen.  
Beim nächsten mal werde ich definitiv Arrange / Act / Assert Kommentare zum unterteilen verwenden.

#### Zu folgenden Punkten konnte ich aus dem Buch etwas mitnehmen:  
- Web-APIs
- Clean code
- EF Core
- Unit tests
- Repository/service pattern  

## FlyingDutchmanAirlines API Specification

### The following endpoints are available:
#### Bookings
- **POST** */Bookings/{flightNumber}* - Create a new booking for a specific flight number
  - Request Body: BookingData
- **POST** */Bookings* - Retrieve bookings for a specific customer
  - Request Body: BookingData
- **GET** */Bookings/{bookingId}* - Retrieve a specific booking
- **DELETE** */Bookings/{bookingId}* - Delete a specific booking

#### Flights
- **GET** */Flights* - Retrieve a list of available flights
- **GET** */Flights/{flightNumber}* - Retrieve a specific flight

#### Schemas
- BookingData - Input for creating a new booking or retrieving bookings for specific customer
- BookingView - The schema for returning a booking
- FlightView - The schema for returning a flight
