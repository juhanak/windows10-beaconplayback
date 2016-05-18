# BeaconPlayback for Windows 10 #
This tool can be used to emit a sequence of beacon events. The main purpose of
the tool is to ease testing of the applications that listen to Ble beacons.

![BeaconPlayback running on Windows](/Screenshots/Screenshot.png)&nbsp;

BeaconPlayback supports two different modes: local and remote. In the local 
mode, a list of the beacons is loaded from the JSON file in the application. 
In the remote mode, a list of beacon events is downloaded from the server.

### Local mode ###

In the local mode repeat count, beacons, signal duration and a delay before 
the next event are specified in the JSON file.

Sample JSON file for the local mode:

```javascript
{"settings": {"mode":"local", "repeat": 2},
"events":[
    {"id1":"43676723-7400-0000-ffff-0000ffff0007", "id2":"24888", "id3":"23777", "duration":2,"sleep":1,},
    {"id1":"43676723-7400-0000-ffff-0000ffff0007", "id2":"24888", "id3":"23777", "duration":2,"sleep":1,}
]}
```

| Element | Value/Values | Document |
| ------- | ------------ | -------- |
| mode    | remote/local | Indicates if the settings file is for local or remote mode. Local settings file needs to include events element|
| repeat  | > 0          | How many times the beacon sequence is replayed |
| events  | > 0 objects  | Each event object needs to have id1, id2, id3, duration and sleep attributes |
| duration| > 0          | Duration in seconds |
| sleep   | > 0          | Seconds before the next event |

### Remote mode ###

In the remote mode, settings file just tells from where to download the list
of beacon events.

Example of the settings file:

```javascript
{"settings": {"mode":"remote", "url": "http://server.com/events.JSON"}}
```

The JSON file on the server contains the list of the events:

```javascript
{"settings": {"repeat": 2},
"events":[
    {"id1":"43676723-7400-0000-ffff-0000ffff0007", "id2":"24888", "id3":"23777", "duration":2,"sleep":1,},
    {"id1":"43676723-7400-0000-ffff-0000ffff0007", "id2":"24888", "id3":"23777", "duration":2,"sleep":1,}
]}
```

If the repeat mode is set on in the application, then when the new 
repeat round starts client checks if the file on the server has been updated.