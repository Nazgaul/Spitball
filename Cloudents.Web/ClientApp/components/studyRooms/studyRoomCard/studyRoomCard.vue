<template>
    <v-container class="study-card-container">
        <v-layout class="study-card-upper-area" :class="{'study-card-active': isActive}">
            <v-flex>
                {{roomStatus}}
            </v-flex>
        </v-layout>
        <div class="study-card-avatar-area">
            <user-avatar :user-name="card.name" :userImageUrl="card.image" :size="'64'"/>
            <span v-if="isOnline" class="online-circle"></span>
        </div>
        <v-layout column align-center justify-space-between class="study-card-lower-area">
            <v-flex py-1 class="study-card-name">
                {{card.name}}
            </v-flex>
            <v-flex py-1 class="study-card-message">
                <v-icon>sbf-message-icon</v-icon>
            </v-flex>
            <v-flex py-1 class="study-card-enter-container">
                <v-icon @click="enterRoom" class="study-card-enter-icon">sbf-enter-icon</v-icon>
                <span @click="enterRoom" class="study-card-enter-text">enter room</span> 
            </v-flex>
        </v-layout>
        <v-layout align-center row justify-space-between class="study-card-created-container">
            <span>
                created
            </span>
            <span>
                {{date}}
            </span>
        </v-layout>
    </v-container>
</template>

<script>
import UserAvatar from "../../helpers/UserAvatar/UserAvatar.vue";
import UserRank from "../../helpers/UserRank/UserRank.vue";
import utilitiesService from "../../../services/utilities/utilitiesService"
export default {
    components:{
        UserAvatar,
        UserRank
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
        enterRoom(){
            let routeData = this.$router.resolve({
                    name: 'tutoring',
                    params: {
                        id:this.card.id
                        }
                });
            global.open(routeData.href, '_blank');
        },
        // sendMessage(){
        //         if ( this.accountUser == null) {
        //             this.updateLoginDialogState(true);
        //         } else {
        //             let currentConversationObj = chatService.createActiveConversationObj(this.tutorData)
        //             this.setActiveConversationObj(currentConversationObj);
        //             let isMobile = this.$vuetify.breakpoint.smAndDown;
        //             if(isMobile){
        //                 //move to chat tab
        //                 this.changeFooterActiveTab('promotions');
        //             }else{
        //                 this.openChatInterface();
        //             }
                    
        //         }
        //     }
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
        min-width: 164px;
        max-width: 164px;
        @media (max-width: @screen-xs) {
            margin:5px;
        }
        .study-card-upper-area{
            background-color: #f0f0f7;
            color:#a5a4bf;
            border-radius: 4px 4px 0 0;
            padding: 16px 0 43px 0;
            text-align: center;
            &.study-card-active{
                background-color: rgba(66, 224, 113, 0.16);
                color: #34ca61;
            }
        }
        .study-card-avatar-area{
            display: flex;
            justify-content: center;
            margin-top: -32px;
            position: relative;
            .online-circle{
                border-radius: 50%;
                width: 10px;
                height: 10px;
                background-color: #00ff14;
                position: absolute;
                bottom: 0;
                right: 57px;
            }
        }
        .study-card-lower-area{
            margin:10px 0;
            color: #5d5d5d;
            font-size:12px;
            letter-spacing: -0.3px;
            margin: 0 12px;
            border-bottom: solid 1px rgba(67, 66, 93, 0.18);
            .study-card-enter-container{
                .study-card-enter-icon{
                    vertical-align: middle;
                    font-size: 14px;
                    cursor: pointer;
                }
                .study-card-enter-text{
                    cursor: pointer;
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
                    font-size:13px;
                    color: #fff;
                    height: 32px;
                    width: 32px;
                    background-color: #43425d;
                    border-radius: 50%;
                    margin: 0 auto;
                    display: flex;
                    cursor: pointer;
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
