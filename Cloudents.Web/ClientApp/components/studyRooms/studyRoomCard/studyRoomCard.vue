<template>
    <div class="study-card-container cursor-pointer" @click="enterRoom">
        <v-layout class="study-card-upper-area" :class="{'study-card-active': isActive}">
            <!-- <v-flex>
                {{roomStatus}}
            </v-flex> -->
        </v-layout>
        <div class="study-card-avatar-area">
            <user-avatar :user-name="card.name" :userImageUrl="card.image" :size="'64'"/>
            <!-- <span v-if="isOnline" class="online-circle"></span> -->
            <userOnlineStatus class="user-status" :userId="card.userId"></userOnlineStatus>
        </div>
        <v-layout column align-center justify-space-between class="study-card-lower-area">
            <v-flex py-1 class="study-card-name">
                {{card.name}}
            </v-flex>
            <v-flex py-1 class="study-card-message">
                <v-icon @click.stop="sendMessage">sbf-message-icon</v-icon>
            </v-flex>
            <v-flex py-1 pb-2 class="study-card-enter-container">
                <v-icon class="study-card-enter-icon mr-1">sbf-enter-icon</v-icon>
                <span class="study-card-enter-text" v-language:inner>studyRoom_enter_room</span>
            </v-flex>
        </v-layout>
        <v-layout align-center row justify-space-between class="study-card-created-container">
            <span v-language:inner>studyRoom_created</span>
            <span>
                {{date}}
            </span>
        </v-layout>
    </div>
</template>

<script>
import {mapActions} from "vuex"
import UserAvatar from "../../helpers/UserAvatar/UserAvatar.vue";
import userOnlineStatus from "../../helpers/userOnlineStatus/userOnlineStatus.vue";
import utilitiesService from "../../../services/utilities/utilitiesService"
import chatService from "../../../services/chatService"
export default {
    components:{
        UserAvatar,
        userOnlineStatus
    },
    props:{
        card:{
            type:Object,
            required: true
        }
    },
    data(){
        return {
            userName: "Ben Stern",
            createdDate: ""
        }
    },
    methods:{
        ...mapActions(['setActiveConversationObj', 'openChatInterface', 'accountUser']),
        enterRoom(){
            let routeData = this.$router.resolve({
                    name: 'tutoring',
                    params: {
                        id:this.card.id
                        }
                });
            global.open(routeData.href, '_blank');
        },
        sendMessage(){
                
                let currentConversationObj = chatService.createActiveConversationObj(this.card)
                this.setActiveConversationObj(currentConversationObj);
                this.openChatInterface();
            }
    },
    computed:{
        date(){
            return utilitiesService.dateFormater(this.card.dateTime);
        },
        isOnline(){
            return this.card.online
        },
        isActive(){
            return false
        },
        roomStatus(){
            if(this.isActive){
                return "Active Room"
            }else{
                return "No Activity yet"
            }
        }
    }
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
    .study-card-container{
        background-color: #fff;
        border-radius: 4px;
        box-shadow: 0 3px 14px 0 rgba(0, 0, 0, 0.36);
        padding: 0;
        margin: 16px 16px 16px 0;
        width: 164px;
        display: flex;
        flex-direction: column;
        @media (max-width: @screen-xs) {
            margin:6px;
            /*&:last-child{*/
                /*align-self: flex-end;*/
            /*}*/
        }
        .study-card-upper-area{
            background-color: #f0f0f7;
            //color:#a5a4bf;
            border-radius: 4px 4px 0 0;
           // padding: 24px 0 43px 0;
            height: 68px;
            // text-align: center;
            // &.study-card-active{
            //     background-color: rgba(66, 224, 113, 0.16);
            //     color: #34ca61;
            // }
        }
        .study-card-avatar-area{
            display: flex;
            justify-content: center;
            margin-top: -32px;
            position: relative;
            .user-status{
                position: absolute;
                bottom: 0;
                right: 58px;
            }
        }
        .study-card-lower-area{
            margin:10px 0;
            color: #5d5d5d;
            font-size:12px;
            letter-spacing: -0.3px;
            margin: 4px 12px;
            border-bottom: solid 1px rgba(67, 66, 93, 0.18);
            .study-card-enter-container{
                .study-card-enter-icon{
                    vertical-align: middle;
                    font-size: 16px;
                }
                .study-card-enter-text{
                    font-weight: bold;
                    text-transform: capitalize;
                }
            }
            .study-card-name{
                text-overflow: ellipsis;
                overflow: hidden;
                white-space: nowrap;
                font-size: 12px;
                color: #5d5d5d;
                font-weight: bold;
            }
            .study-card-message{
                .sbf-message-icon{
                    font-size:14px;
                    color: #fff;
                    height: 32px;
                    width: 32px;
                    background-color: @global-purple;
                    border-radius: 50%;
                    margin: 0 auto;
                    display: flex;
                    //cursor: pointer;
                    padding-top: 3px;
                }
            }
        }
        .study-card-created-container{
            font-size: 11px;
            color: rgba(0, 0, 0, 0.54);
            padding: 8px 12px
        }
    }
</style>
