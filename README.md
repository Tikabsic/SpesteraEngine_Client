<h1 align="center" id="title">SpesteraEngine Client</h1>

<p id="description">Spestera Engine Client is a client-side application designed for connecting to the Spestera Engine server infrastructure. It is built using C# and Unity for a seamless integration with the game server supporting features for multi-player scenarios. The client communicates with the server to handle gameplay mechanics player interactions and real-time updates.</p>

  
  
<h2>Features</h2>

Here're some of the project's best features:

*   Player Movement: Smooth handling of player movement with Unity's \`CharacterController\` and real-time updates.
*   Networking: Asynchronous communication with the server using TCP for game state updates and UDP for real-time messages.
*   Protobuf Integration: Utilizes Google Protocol Buffers for efficient serialization and deserialization of data sent between the client and server.
*   Camera Controls: Handles camera movement and orientation based on player input for a dynamic gaming experience.
*   Game State Management: Manages and synchronizes game state player data and interactions with the server.

<h2>Installation Steps:</h2>

<p>1. Dependencies</p>

- **Unity:** The game engine used for building the client application.
- **Cinemachine:** For advanced camera managing.
- **Google Protobuf:** For efficient data serialization and communication.
- **Zlib:** For data compression.

<p>2. Clone the repository</p>

```
git clone https://github.com/Tikabsic/SpesteraEngine_Client.git
```

<p>3. Clone the repository</p>

* Launch Unity Hub.
* Click "Add" and select the cloned repository folder.

<p>4. Install Dependencies</p>

* install Cinemachine

* install TextMeshPro

* put this in first line of manifest.json file.
```
    "com.gameworkstore.googleprotobufunity": "https://github.com/GameWorkstore/google-protobuf-unity.git#3.15.2012",
```

<h2>Building</h2>

<p>Open the Unity Editor: </p>

* Open the project using Unity Editor.
<p>Build the Project:</p>

* Go to File > Build Settings.
* Select the target platform (e.g., Windows, macOS, Linux).
* Click Build to compile the project.

<h2> Usage </h2>

<h3>1. Run the Server</h3>

 * Ensure that the Spestera Engine server is running.
* Configure the server address and port settings in the Unity Editor or via configuration files.

<h3>2. Run the Client</h3>

* Execute the built application or run it from within the Unity Editor.
* Use the input controls to interact with the game and connect to the server.

<h2> Future Plans </h2>

<h3> Future client architecture</h3>

* **Enhanced Networking:** Improve network performance and reliability with advanced techniques for handling latency and packet loss.

* **Graphics and UI Upgrades:** Incorporate advanced graphics features and enhance user interface elements for a more immersive experience.

* **Gameplay Enhancements:** Develop and integrate additional gameplay features.

* **Security Improvements:** Implement additional security measures to protect against potential threats and ensure safe gameplay.
