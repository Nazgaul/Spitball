<template>
    <v-layout row class="tutoring-page" :class="{'gridBackground': $route.name === 'tutoring'}">
        <v-flex>
            <v-tabs v-model="activeTab">
                <v-tab v-for="n in tabs"
                       class="tutoring-tab"
                       :key="n.name"
                       ripple>
                    {{n.name}}
                </v-tab>
                <v-tab-item :key="1" >
                    <white-board></white-board>
                </v-tab-item>
                <v-tab-item :key="2" >
                    <codeEditor v-show="isRoomCreated"></codeEditor>
                </v-tab-item>
            </v-tabs>
        </v-flex>
        <v-layout column align-start style="position: fixed; right: 0; top: 44px;">
            <v-flex xs6 sm6 md6>
                <video-stream :id="id"></video-stream>
            </v-flex>
        </v-layout>
        <v-layout column align-end style="position: fixed; right: 24px; bottom: 12px;">
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
    export default {
        components:{videoStream, whiteBoard, codeEditor, chat},
        name: "tutor",
        data() {
            return {
                activeTab: '',
                tabs: [ {name: 'Whiteboard'}, {name:'Code Collaboration'}]
            }
        },
        props: {
            id: ''
        },
        computed: {
            ...mapGetters(['isRoomCreated']),
        },

        created() {
            console.log('ID Tutor!!',this.id);
            global.onbeforeunload = function(){
                return "Are you sure you want to close the window?";
            }
        }
    }
</script>

<style lang="less">
    .tutoring-page{
        .tutoring-tab{
            .v-tabs__item{
                text-transform: capitalize;
            }
        }

        /*rtl:ignore*/
        direction: ltr;
        height: 100%;
        &.gridBackground{
            background-color:  #F8F8F8;
            background-size: 40px 40px;
            background-image: linear-gradient(to right, grey 1px, transparent 1px), linear-gradient(to bottom, grey 1px, transparent 1px);
            background-repeat: initial;
            background-blend-mode: color-burn;
        }
    }

</style>