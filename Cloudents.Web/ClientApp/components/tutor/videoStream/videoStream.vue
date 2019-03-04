<template>
    <v-container class="videos-wrapper py-0">
        <v-layout>
            <v-flex>
                <div class="roomTitle">
                    <span v-if="loading"> Loading... {{roomName}}</span>
                    <span v-else-if="!loading && roomName"> Connected to {{roomName}}</span>
                    <span v-else>Generate a room to get started</span>
                </div>
            </v-flex>
        </v-layout>
        <v-layout>
            <v-flex>
                <v-btn @click="generateRoom()" v-if="!id" primary>Generate Room</v-btn>
                <v-btn @click="doCopy" v-if="id">{{isCopied ? 'Copied' : 'Copy ID'}}</v-btn>
                <!--<v-btn @click="sendData()">Click</v-btn>-->
            </v-flex>
        </v-layout>

        <v-layout column align-end>
            <div class="video-holder">
                <v-flex>
                    <span class="video-size-ctrl" @click="minimize('local_player')">{{visible.local_player ? "Minimize" : "Maximize"}}</span>
                    <span class="video-size-ctrl" @click="biggerLocalVideo">{{visible.local_player ? "Full" : "Not full"}}</span>
                    <span class="video-size-ctrl" @click="requestPictureInPicture('localTrack')">Picture mode</span>
                </v-flex>
                <v-flex v-show="visible.local_player">
                    <div class="row">
                        <!--<h4>Tutor {{members[0].identity}}</h4>-->
                        <div id="localTrack"></div>
                    </div>
                </v-flex>
            </div>
            <div class="video-holder">
                <v-flex>
                    <span class="video-size-ctrl" @click="minimize('remote_player')">{{visible.remote_player ? "Minimize" : "Maximize"}}</span>
                    <span class="video-size-ctrl" @click="biggerRemoteVideo">{{visible.local_player ? "Full" : "Not full"}}</span>
                    <span class="video-size-ctrl" @click="requestPictureInPicture('remoteTrack')">Picture mode</span>
                </v-flex>
                <v-flex v-show="visible.remote_player">
                    <div class="row remote_video_container">
                        <!--<h4>Student {{members[1].identity}}</h4>-->
                        <div id="remoteTrack" ref='remote_player'></div>
                    </div>
                </v-flex>
            </div>

        </v-layout>
    </v-container>
</template>

<script>
    import { mapActions } from 'vuex';
    import Twilio, { connect, createLocalTracks, createLocalVideoTrack, LocalDataTrack } from 'twilio-video';
    import videoService from '../../../services/videoStreamService';
    import { dataTrack } from '../tutorService';
    import whiteBoardService from '../whiteboard/whiteBoardService'


    export default {
        name: "videoStream",
        data() {
            return {
                loading: false,
                data: {},
                isCopied: false,
                localTrackAval: false,
                remoteTrack: '',
                activeRoom: '',
                previewTracks: '',
                identity: '',
                roomName: '',
                roomLink: '',
                username: '',
                members: ['', ''],
                availableDevices: [],
                visible: {
                    'local_player': true,
                    'remote_player': true
                }
            }
        },
        props: {
            id: ''
        },
        watch: {
            '$route': 'createChat'
        },
        methods: {
            ...mapActions(['addMessage', 'updateUserIdentity']),
            biggerLocalVideo() {
                let video = document.querySelectorAll("#localTrack video")[0];
                video.requestFullscreen()
            },
            biggerRemoteVideo() {
                let video = document.querySelectorAll("#remoteTrack video")[0];
                video.requestFullscreen()
            },
            requestPictureInPicture(videoType) {
                let video = document.querySelectorAll(`#${videoType} video`)[0];
                video.requestPictureInPicture();
            },
            minimize(type) {
                this.visible[`${type}`] = !this.visible[`${type}`];
            },
            // sendData() {
            //     let joy = {test: 'Hello Beny DATA!!!!'};
            //     dataTrack.send(JSON.stringify(joy));
            // },
            generateRoom() {
                let self = this;
                videoService.generateRoom().then(data => {
                    self.roomLink = data.data.name;
                    self.$router.push({name: 'tutoring', params: {id: self.roomLink}});
                })

            },
            startChat() {
                this.createChat()
            },
            doCopy() {
                let self = this;
                this.$copyText(self.roomLink).then((e) => {
                    self.isCopied = true;
                }, (e) => {
                })
            },

            // Generate access token
            async getAccessToken() {
                return await videoService.getToken(this.id);
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
                    if (track.detach) {
                        track.detach().forEach((detachedElement) => {
                            detachedElement.remove();
                        });
                    }
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
                        let connectOptions;
                        createLocalTracks({
                            audio: self.availableDevices.includes('audioinput'),
                            video: self.availableDevices.includes('videoinput') ? {width: 350, height: 200} : {},
                        }).then((tracksCreated) => {
                            let localMediaContainer = document.getElementById('localTrack');
                            tracksCreated.forEach((track) => {
                                localMediaContainer.appendChild(track.attach());
                                self.localTrackAval = true;
                            });
                            if (self.availableDevices) {
                                tracksCreated.push(dataTrack);
                                connectOptions = {
                                    tracks: tracksCreated
                                }
                            }
                            self.connect(token, connectOptions);

                        }, (error) => {
                            console.log(error, 'error create tracks before connect')
                        })

                    })
                    .catch(function (err) {
                        console.log(err.name + ": " + err.message);
                    });
            },

            connect(token, options) {
                let self = this;
                // disconnect the user from they joined already
                self.leaveRoomIfJoined();
                Twilio.connect(token, options)
                    .then((room) => {
                            console.log('Successfully joined a Room: ', 'dfgdfg');
                            // set active toom
                            self.activeRoom = room;
                            self.roomName = room.name;
                            self.loading = false;
                            let localIdentity = room.localParticipant &&  room.localParticipant.identity ? room.localParticipant.identity : ''
                            self.updateUserIdentity(localIdentity);
                            // Attach the Tracks of all the remote Participants.
                            self.activeRoom.participants.forEach((participant, index) => {
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
                                        let previewContainer = document.getElementById('remoteTrack');
                                        console.log('remote track attached', " added track: " + track.kind)
                                        self.attachTracks([track], previewContainer);
                                    }
                                });
                            });
                            // When a Participant adds a Track, attach it to the DOM.
                            room.on('trackSubscribed', (track, participant) => {
                                if (track.kind === 'data') {
                                    track.on('message', transferObj => {
                                        // console.log(`Mouse coordinates: (${transferObj})`);
                                        let Data = JSON.parse(transferObj);
                                        let parsedData = Data.data;
                                        if (Data.type === 'redrawData') {
                                            whiteBoardService.redraw(parsedData);
                                        } else if (Data.type === 'undoData') {
                                            whiteBoardService.undo(parsedData);
                                        }
                                        else if (Data.type === 'tutoringChatMessage') {
                                            console.log('chat message', Data);
                                            this.addMessage(Data);
                                        }
                                    });
                                }
                                let previewContainer = document.getElementById('remoteTrack');
                                console.log('track attached', " added track: " + track.kind);
                                self.attachTracks([track], previewContainer);
                            });
                            // When a Participant's Track is unsubscribed from, detach it from the DOM.
                            room.on('trackUnsubscribed', function (track) {
                                console.log(" removed track: " + track.kind);
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
                                let localTracksOptions = {
                                    logLevel: 'debug',
                                    audio: self.availableDevices.includes('audioinput'),
                                    // video: self.availableDevices.includes('videoinput'),
                                    video: self.availableDevices.includes('videoinput') ? {width: 350, height: 200} : {},
                                };
                                createLocalTracks(localTracksOptions)
                                    .then(tracks => {
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
                // remove any remote track when joining a new room
                let clearEl = document.getElementById('remoteTrack');
                if (clearEl) {
                    clearEl.innerHTML = "";
                }
                self.isHardawareAvaliable();
            },
        },
        created() {
            if (this.id) {
                this.startChat();
            }
        }

    }
</script>

<style lang="less" src="./videoStream.less">

</style>