<template>
    <div>
        <v-bottom-navigation
                height="48px"
                
                :value="activeBtn"
                fixed
                color="#f5f5f5"
                :app="$vuetify.breakpoint.xsOnly && getMobileFooterState()"
                class="notransition mob-footer-navigation"
        >
            <v-btn text color="teal" value="feed" @click="changeActiveTab(tabs.feed)">
                <span class="mob-footer-title" v-language:inner>mobileFooter_action_feed</span>
                <v-icon class="mob-footer-icon" v-if="activeBtn !== tabs.feed">sbf-icon-feed</v-icon>
                <v-icon class="mob-footer-icon" v-else>sbf-icon-feed-selected</v-icon>

            </v-btn>
            <v-btn text color="teal" value="earners"  @click="changeActiveTab(tabs.earners)">
                <span class="mob-footer-title" v-language:inner>mobileFooter_action_earners</span>
                <v-icon class="mob-footer-icon" v-if="activeBtn !== tabs.earners">sbf-graduation</v-icon>
                <v-icon class="mob-footer-icon" color="#4452FC" v-else>sbf-graduation</v-icon>
            </v-btn>

            <!-- <v-btn flat color="teal" value="promotions"  @click="changeActiveTab(tabs.promotions)"> -->
            <v-btn text color="teal" value="promotions"  @click="openChat()">
                <span class="mob-footer-title" v-language:inner>mobileFooter_action_promotion</span>
                <v-icon class="mob-footer-icon" v-if="activeBtn !== tabs.promotions">sbf-message-icon</v-icon>
                <!-- <v-icon class="mob-footer-icon" v-else>sbf-icon-promotions-selected</v-icon> -->
                <span class="unread-circle" v-show="totalUnread > 0">{{totalUnread}}</span>
            </v-btn>
        </v-bottom-navigation>
    </div>
</template>
<script>
    import { mapActions, mapGetters } from 'vuex';

    export default {
        name: "mobileFooter",
        props: {
            onStepChange: {
                type: Function,
                required: false
            },
        },
        data() {
            return {
                // activeBtn: 'feed',
                showNav: true,
                tabs: this.getFooterEnumsState(),
                lastTab: null,
            }
        },

        computed: {
            ...mapGetters(['getTotalUnread', 'accountUser', 'getCurrentActiveTabName']),
            totalUnread(){
                return this.getTotalUnread
            },
            activeBtn:{
                get(){
                    return this.getCurrentActiveTabName;
                },
                set(val){
                    this.changeFooterActiveTab(val)
                }
            }
        },
        watch:{
            activeBtn(newVal, oldVal){
                this.lastTab = oldVal;
            }
        },
        methods: {
            ...mapActions(['changemobileMarketingBoxState', 'changeFooterActiveTab', 'updateLoginDialogState','openChatInterface']),
            ...mapGetters(["getFooterEnumsState", "getMobileFooterState"]),
            openChat(){
                if (this.accountUser == null) {
                    this.updateLoginDialogState(true);
                    setTimeout(()=>{ 
                        this.changeFooterActiveTab(this.lastTab);
                    }, 200)
                }else{
                    this.openChatInterface();
                    setTimeout(()=>{
                        this.changeFooterActiveTab(this.lastTab);
                    }, 200)
                }
            },
            changeActiveTab(val){
                this.onStepChange();
                this.changeFooterActiveTab(val);
            }
        },
    }
</script>

<style lang="less">
    @import "../../../styles/mixin.less";
    .v-bottom-nav{
        &.mob-footer-navigation{
            box-shadow: none;
            border-top: 1px solid #d7d7d7!important;
        }
    }

    .mob-footer-icon{
        font-size: 20px;
    }
    .unread-circle{
        position: absolute;
        top: -6px;
        right: 37px;
        background: #ce3333;
        color: #fff;
        border-radius: 50%;
        height: 13px;
        width: 13px;
        line-height: 13px;
        display: flex;
        font-size: 10px;
        justify-content: center;
        flex-direction: column;
        text-align: center;
    }
    .v-item-group.v-bottom-nav .v-btn--active .v-btn__content {
        font-size: 12px;
        color: #4452FC;
    }
    .v-item-group.v-bottom-nav .v-btn--active {
        padding-top: 8px;
    }

    .mob-footer-title {
        opacity: 0.9;
        font-weight: 500;
        letter-spacing: -0.1px;
        text-align: center;
    }

</style>