<template>
    <v-layout row class="tutoring-page"
              :style="{'background-size': zoom, 'background-position-x': panX, 'background-position-y': panY}"
              :class="{'gridBackground': $route.name === 'tutoring'} ">
        <v-flex>
            <nav class="tutoring-navigation">
                <div class="logo-nav-wrap">
                    <span class="logo-container">
                        <AppLogo></AppLogo>
                    </span>
                    <div class="tutor-nav-item" v-for="singleNav in navs"
                         :class="{ 'active-nav': singleNav.value === activeItem}">
                        <v-icon class="mr-2">{{singleNav.icon}}</v-icon>
                        <a class="tutor-nav-item-link"
                           @click="updateActiveNav(singleNav.value)">{{singleNav.name}}</a>
                    </div>
                </div>

                <div style="display: flex; align-items: center;">
                    <share-screen-btn class="nav-share-btn"></share-screen-btn>
                    <button class="outline-btn" @click="changeQualityDialogState(true)">
                        <testIcon class="test-icon mr-1"></testIcon>
                        System Check
                    </button>
                    <div class="mr-4 pr-1 d-flex">
                        <component v-if="localNetworkQuality" class="network-icon ml-3" :is="'signal_level_'+localNetworkQuality"></component>
                    </div>
                </div>
            </nav>
            <transition name="slide-x-transition">
                <keep-alive>
                    <component :is="activeItem" v-if="showCurrentCondition"></component>
                </keep-alive>
            </transition>
        </v-flex>
        <v-layout column align-start style="position: fixed; right: 0; top: 60px;">
            <v-flex xs6 sm6 md6>
                <video-stream :id="id"></video-stream>
            </v-flex>
        </v-layout>
        <v-layout column align-end style="position: fixed; right: 24px; bottom: 0px;">
            <v-flex xs6 sm6 md6>
                <chat v-show="isRoomCreated" :id="id"></chat>
            </v-flex>
        </v-layout>

        <v-dialog v-model="qualityDialog" content-class="filter-dialog"
                  :max-width="$vuetify.breakpoint.smAndUp ? '720px' : ''" :fullscreen="$vuetify.breakpoint.xsOnly"
                  persistent>
            <quality-validation></quality-validation>
        </v-dialog>

    </v-layout>


</template>
<script>
    import { mapActions, mapGetters } from 'vuex';
    import videoStream from './videoStream/videoStream.vue';
    import whiteBoard from './whiteboard/WhiteBoard.vue';
    import codeEditor from './codeEditor/codeEditor.vue'
    import chat from './chat/chat.vue';
    import qualityValidation from './tutorHelpers/qualityValidation/qualityValidation.vue'
    import sharedDocument from './sharedDocument/sharedDocument.vue';
    import shareScreenBtn from './tutorHelpers/shareScreenBtn.vue';
    import AppLogo from "../../../wwwroot/Images/logo-spitball.svg";
    import testIcon from './images/eq-system.svg'
    import signal_level_0 from  './images/wifi-0.svg';
    import signal_level_1 from  './images/wifi-1.svg';
    import signal_level_2 from  './images/wifi-2.svg';
    import signal_level_3 from  './images/wifi-3.svg';
    import signal_level_4 from  './images/wifi-4.svg';
    import signal_level_5 from  './images/wifi-4.svg';
    export default {
        components: {
            videoStream,
            whiteBoard,
            codeEditor,
            chat,
            sharedDocument,
            shareScreenBtn,
            AppLogo,
            qualityValidation,
            testIcon,
            signal_level_0,
            signal_level_1,
            signal_level_2,
            signal_level_3,
            signal_level_4,
            signal_level_5,
        },
        name: "tutor",
        data() {
            return {
                activeNavItem: 'white-board',
                showQualityDialog: false,
                showContent: false,
                navs: [
                    {name: 'Canvas', value: 'white-board', icon: 'sbf-canvas'},
                    {name: 'Code Editor', value: 'code-editor', icon: 'sbf-code-editor'},
                    {name: 'Text Editor', value: 'shared-document', icon: 'sbf-text-icon'}
                ],
            }
        },

        props: {
            id: ''
        },
        computed: {
            ...mapGetters([
                'qualityDialog',
                'localNetworkQuality',
                'isRoomCreated',
                'isRoomFull',
                'sharedDocUrl',
                'getZoom',
                'getPanX',
                'getPanY']),
            activeItem() {
                return this.activeNavItem
            },
            showCurrentCondition() {
                return this.activeItem === 'white-board' ? true : this.isRoomCreated
            },
            zoom() {
                let gridSize = 40 * Number(this.getZoom.toFixed()) / 100;
                return `${gridSize}px ${gridSize}px`;
            },
            panX() {
                return `${this.getPanX}px`
            },
            panY() {
                return `${this.getPanY}px`
            },
        },
        methods: {
            ...mapActions(['updateTestDialogState']),
            updateActiveNav(value) {
                this.activeNavItem = value;
                console.log(this.activeItem)
            },
            changeQualityDialogState(val) {
                this.updateTestDialogState(val)
            }
        },
        created() {
            this.$loadScript("https://cdnjs.cloudflare.com/ajax/libs/mathjax/2.7.5/MathJax.js?config=TeX-AMS_SVG").then(() => {
                MathJax.Hub.Config({
                    showMathMenu: false,
                    SVG: {
                        useGlobalCache: false,
                        useFontCache: false,
                    }
                });
                MathJax.AuthorInit = function (texstring, callback) {
                    var input = texstring;
                    var wrapper = document.createElement("div");
                    wrapper.innerHTML = input;
                    var output = {svg: ""};
                    MathJax.Hub.Queue(["Typeset", MathJax.Hub, wrapper]);
                    MathJax.Hub.Queue(function () {
                        var mjOut = wrapper.getElementsByTagName("svg")[0];
                        if (!mjOut) {
                            return null;
                        }
                        mjOut.setAttribute("xmlns", "http://www.w3.org/2000/svg");
                        output.svg = mjOut.outerHTML;
                        callback(output);
                    });
                }
                //MathJax.Message.Log()
            });
            console.log('ID Tutor!!', this.id);
            global.onbeforeunload = function () {
                return "Are you sure you want to close the window?";
            }
        }
    }
</script>

<style lang="less" src="./tutor.less"></style>