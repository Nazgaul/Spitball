<template>
    <v-layout row class="tutoring-page" :style="{'background-size': zoom, 'background-position-x': panX, 'background-position-y': panY}" :class="{'gridBackground': $route.name === 'tutoring'} ">
        <v-flex>
            
            <v-tabs class="tutoring-navigation" v-model="activeTab" touchless hide-slider>
                <span class="logo-container"><AppLogo></AppLogo></span>
                <v-tab v-for="n in tabs"
                       touchless
                       class="tutoring-tab"
                       :key="n.name"
                       ripple>
                    {{n.name}}
                </v-tab>
                <v-tab-item :key="1" >
                    <white-board></white-board>
                </v-tab-item>
                <v-tab-item :key="2" >
                    <codeEditor v-if="isRoomCreated"></codeEditor>
                </v-tab-item>
                <v-tab-item :key="3">
                    <shared-document :key="sharedDocUrl" v-if="isRoomCreated"></shared-document>
                </v-tab-item>
            </v-tabs>
        </v-flex>
        <v-layout column align-start style="position: fixed; right: 0; top: 44px;">
            <v-flex xs6 sm6 md6>
                <video-stream :id="id"></video-stream>
            </v-flex>
        </v-layout>
        <v-layout column align-end style="position: fixed; right: 24px; bottom: 0px;">
            <v-flex xs6 sm6 md6>
                <chat v-show="isRoomCreated" :id="id"></chat>
            </v-flex>
        </v-layout>
    </v-layout>


</template>
<script>
    import {mapGetters} from 'vuex';
    import videoStream from './videoStream/videoStream.vue';
    import whiteBoard from './whiteboard/WhiteBoard.vue';
    import codeEditor from './codeEditor/codeEditor.vue'
    import chat from './chat/chat.vue';
    import sharedDocument from './sharedDocument/sharedDocument.vue';
    import {passSharedDocLink} from './tutorService'
    import AppLogo from "../../../wwwroot/Images/logo-spitball.svg";
    export default {
        components:{videoStream, whiteBoard, codeEditor, chat, sharedDocument, AppLogo},
        name: "tutor",
        data() {
            return {
                activeTab: '',
                tabs: [ {name: 'Whiteboard'}, {name:'Code Collaboration'}, {name: 'Document'}],
            }
        },

        props: {
            id: ''
        },
        computed: {
            ...mapGetters(['isRoomCreated', 'isRoomFull', 'sharedDocUrl', 'getZoom', 'getPanX', 'getPanY']),
            zoom(){
                let gridSize = 40 * Number(this.getZoom.toFixed())/100;
                return  `${gridSize}px ${gridSize}px`;
            },
            panX(){
                return `${this.getPanX}px`
            },
            panY(){
                return `${this.getPanY}px`
            },
        },
        created() {
            console.log('ID Tutor!!',this.id);
            global.onbeforeunload = function(){
                return "Are you sure you want to close the window?";
            }
        }
    }
</script>

<style lang="less" src="./tutor.less"></style>