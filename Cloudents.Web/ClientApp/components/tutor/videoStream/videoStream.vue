<template>
    <v-container>
        <v-layout>
            <v-flex>
                <v-text-field solo type="text" class="form-control" v-model="username"></v-text-field>
                <v-text-field solo type="text" class="form-control" v-model="roomName"></v-text-field>
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
                <div v-for="participant in members">
                    <div>Participants Name</div>
                    <h4>{{participant.identity}}</h4>
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
                localTrack: false,
                remoteTrack: '',
                activeRoom: '',
                previewTracks: '',
                identity: '',
                roomName: 'Room One',
                username: '',
                members: []
            }
        },
        // props: ['username'], // props that will be passed to this component

        methods: {
            startChat() {
                this.createChat(this.roomName)
            },
            // Generate access token
            async getAccessToken() {
                return await videoService.getToken('https://285e7df9.ngrok.io/token', this.username);
            },

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
            // Create a new chat
            createChat(room_name) {
                this.loading = true;
                const self = this;

                this.getAccessToken().then((data) => {
                    self.roomName = null;
                    const token = data.data.token;
                    let connectOptions = {
                        name: room_name,
                        logLevel: 'debug',
                        audio: true,
                        video: {width: 400}
                    };
                    // before a user enters a new room,
                    // disconnect the user from they joined already
                    this.leaveRoomIfJoined();

                    // remove any remote track when joining a new room
                    document.getElementById('remoteTrack').innerHTML = "";

                    Twilio.connect(token, connectOptions)
                        .then((room) => {
                                // console.log('Successfully joined a Room: ', room);
                                console.log('Successfully joined a Room: ' + room_name, 'dfgdfg', room);

                                // set active toom
                                self.activeRoom = room;
                                self.roomName = room_name;
                                self.loading = false;

                                // Attach the Tracks of all the remote Participants.
                                room.participants.forEach((participant) => {
                                    let previewContainer = document.getElementById('remoteTrack');
                                    self.attachParticipantTracks(participant, previewContainer);
                                });
                                // When a Participant joins the Room, log the event.
                                room.on('participantConnected', (participant) => {
                                    self.members.push(participant);
                                    console.log('user connected', participant.identity)
                                });
                                // When a Participant adds a Track, attach it to the DOM.
                                room.on('trackSubscribed', (track, participant) => {
                                    let previewContainer = document.getElementById('remoteTrack');
                                    console.log('track attached', " added track: " + track.kind)
                                    self.attachTracks([track], previewContainer);
                                });
                                // When a Participant removes a Track, detach it from the DOM.
                                room.on('trackRemoved', (track, participant) => {
                                    console.log(participant.identity + " removed track: " + track.kind);
                                    self.detachTracks([track]);
                                });
                                // When a Participant leaves the Room, detach its Tracks.
                                room.on('participantDisconnected', (participant) => {
                                    console.log("Participant '" + participant.identity + "' left the room");
                                    self.detachParticipantTracks(participant);
                                });
                                // if local preview is not active, create it
                                if (!self.localTrack) {
                                    createLocalVideoTrack().then(track => {
                                        let localMediaContainer = document.getElementById('localTrack');
                                        localMediaContainer.appendChild(track.attach());
                                        self.localTrack = true;
                                    });
                                }

                            },
                            (error) => {
                                console.log(error, 'error cant connect')
                            });
                })
            },
        },
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