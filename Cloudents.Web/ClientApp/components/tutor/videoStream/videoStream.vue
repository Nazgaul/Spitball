<template>
    <v-container>
        <v-layout>
            <v-flex>
                <!--<v-text-field solo type="text" class="form-control" v-model="username"></v-text-field>-->
                <!--<v-text-field solo type="text" class="form-control" v-model="roomName"></v-text-field>-->
                <v-btn @click="generateRoom()" primary>Generate Room</v-btn>
                <div>{{roomLink}}</div>
            </v-flex>
        </v-layout>
        <v-layout>
            <v-flex>
                <v-btn @click="startChat()" primary>Submit</v-btn>
            </v-flex>
        </v-layout>
        <v-layout>
            <v-flex>
                <div class="roomTitle">
                    <span v-if="loading"> Loading... {{roomName}}</span>
                    <span v-else-if="!loading && roomName"> Connected to {{roomName}}</span>
                    <span v-else>Select a room to get started</span>
                </div>
            </v-flex>
        </v-layout>
        <v-layout row>
            <v-flex>
                <div class="row remote_video_container">
                    <div id="remoteTrack"></div>
                    <h4>sdfsdfsd</h4>
                </div>
            </v-flex>
            <v-flex>
                <div v-for="member in members">
                    <div>Participants Name</div>
                    <h4>{{member.identity}}</h4>
                </div>
            </v-flex>
        </v-layout>
        <v-layout>
            <v-flex>
                <div class="spacing"></div>
                <div class="row">
                    <div id="localTrack"></div>
                </div>
            </v-flex>
        </v-layout>


    </v-container>
</template>

<script>
    import Twilio, { connect, createLocalTracks, createLocalVideoTrack } from 'twilio-video';
    import videoService from '../../../services/videoStreamService';

    export default {
        name: "videoStream",
        data() {
            return {
                loading: false,
                data: {},
                localTrackAval: false,
                remoteTrack: '',
                activeRoom: '',
                previewTracks: '',
                identity: '',
                roomName: 'Room One',
                roomLink: '',
                username: '',
                members: [],
                availableDevices: []
            }
        },
        props: {
            id: ''
        }, // props that will be passed to this component

        methods: {
            generateRoom() {
                let self = this;
                console.log("swefrkhjslfjslf")
                videoService.generateRoom().then( data  => {
                    self.roomLink = data.data.name
                })
               
                
            },
            startChat() {
                this.createChat(this.roomName)
            },
            // Generate access token
            async getAccessToken() {
                return await videoService.getToken(this.id);
            },

            //async generateRoom() {
            //    return await videoService.generateRoom();
            //},

            // Trigger log events
            dispatchLog(message) {
            },

            // Attach the Tracks to the DOM.
            attachTracks(tracks, container) {
                tracks.forEach((track) => {
                    container.appendChild(track.attach());
                });
            },

            // Attach the Participant's Tracks to the DOM.
            attachParticipantTracks(participant, container) {
                let tracks = Array.from(participant.tracks.values());
                this.attachTracks(tracks, container);
            },

            // Detach the Tracks from the DOM.
            detachTracks(tracks) {
                tracks.forEach((track) => {
                    track.detach().forEach((detachedElement) => {
                        detachedElement.remove();
                    });
                });

            },

            // Detach the Participant's Tracks from the DOM.
            detachParticipantTracks(participant) {
                let tracks = Array.from(participant.tracks.values());
                this.detachTracks(tracks);
            },
            // Leave Room.
            leaveRoomIfJoined() {
                if (this.activeRoom) {
                    this.activeRoom.disconnect();
                }
            },

            //delete members from array when left
            removeMember(mem) {
                let index = this.members.indexOf(mem);
                this.members.splice(index, 1);
            },
            async isHardawareAvaliable() {
                let self = this;
                if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
                    console.log("enumerateDevices() not supported.");
                    return;
                }
                const token = await self.getAccessToken();
                // List cameras and microphones.
                navigator.mediaDevices.enumerateDevices()
                    .then(function (devices) {
                        devices.forEach(function (device) {
                            console.log(device.kind + ": " + device.label +
                                " id = " + device.deviceId);
                            self.availableDevices.push(device.kind);
                        });
                       // global.room;// data.data.token;
                        let connectOptions;
                        if (self.availableDevices) {
                            connectOptions = {
                                audio: self.availableDevices.includes('audioinput'),
                                video: self.availableDevices.includes('videoinput')
                            }
                        }
                        // let connectOptions = {
                        //     // name: room_name,
                        //     // logLevel: 'debug',
                        //     audio: true,
                        //     video: true
                        // };
                        self.connect(token, connectOptions);

                    })
                    .catch(function (err) {
                        console.log(err.name + ": " + err.message);
                    });
            },
            connect(token, options) {
                let self = this;
                Twilio.connect(token, options)
                    .then((room) => {
                        // console.log('Successfully joined a Room: ', room);
                        console.log('Successfully joined a Room: ' + room, 'dfgdfg');

                        // set active toom
                        self.activeRoom = room;
                        self.roomName = room.name;
                        self.loading = false;
                        // Attach the Tracks of all the remote Participants.
                        room.participants.forEach((participant) => {
                            let previewContainer = document.getElementById('remoteTrack');
                            self.members.push(participant);
                            self.attachParticipantTracks(participant, previewContainer);
                        });
                        // Attach the Participant's Media to a <div> element.
                        room.on('participantConnected', participant => {
                            console.log(`Participant "${participant.identity}" connected`);
                            self.members.push(participant);
                            participant.tracks.forEach(publication => {
                                if (publication.isSubscribed) {
                                    const track = publication.track;
                                    document.getElementById('remoteTrack').appendChild(track.attach());
                                }
                            });

                            participant.on('trackSubscribed', track => {
                                document.getElementById('remoteTrack').appendChild(track.attach());
                            });
                        });
                        // When a Participant adds a Track, attach it to the DOM.
                        // room.on('trackSubscribed', (track, participant) => {
                        //     let previewContainer = document.getElementById('remoteTrack');
                        //     console.log('track attached', " added track: " + track.kind)
                        //     self.attachTracks([track], previewContainer);
                        // });
                        // When a Participant removes a Track, detach it from the DOM.
                        room.on('trackUnsubscribed', (track, participant) => {
                            console.log(participant.identity + " removed track: " + track.kind);
                            self.detachTracks([track]);
                        });
                        // When a Participant leaves the Room, detach its Tracks.
                        room.on('participantDisconnected', (participant) => {
                            self.removeMember(participant);
                            console.log("Participant '" + participant.identity + "' left the room");
                            self.detachParticipantTracks(participant);
                        });
                        // if local preview is not active, create it
                        if (!self.localTrackAval) {
                            console.log('11local tracks got here')
                            // let localTracksOptions = {
                            //     // name: room_name,
                            //     logLevel: 'debug',
                            //     audio: false,
                            //     video: false
                            // };
                            createLocalTracks()
                                .then(tracks => {
                                    console.log('222 inside than local tracks got here')
                                    let localMediaContainer = document.getElementById('localTrack');
                                    tracks.forEach((track) => {
                                        localMediaContainer.appendChild(track.attach());
                                        self.localTrackAval = true;
                                    })
                                },
                                    (error) => {
                                        console.error('Unable to access local media video and audio', error);
                                    }
                                );
                        }

                    },
                        (error) => {
                            console.log(error, 'error cant connect')
                        });
            },
            // Create a new chat
            createChat() {
                this.loading = true;
                const self = this;
                self.isHardawareAvaliable();
                // self.roomName = null;


                // before a user enters a new room,
                // disconnect the user from they joined already
                self.leaveRoomIfJoined();
                // remove any remote track when joining a new room
                document.getElementById('remoteTrack').innerHTML = "";

            },
        },
        created() {
            console.log('ID VIDEO!!', this.id)
        }
        
    }
</script>

<style scoped lang="less">
    .remote_video_container {
        left: 0;
        margin: 0;
        border: 1px solid rgb(124, 129, 124);
    }

    #localTrack video {
        border: 3px solid rgb(124, 129, 124);
        margin: 0px;
        max-width: 50% !important;
        background-repeat: no-repeat;
    }

    .spacing {
        padding: 20px;
        width: 100%;
    }

    .roomTitle {
        border: 1px solid rgb(124, 129, 124);
        padding: 4px;
        color: dodgerblue;
    }
</style>