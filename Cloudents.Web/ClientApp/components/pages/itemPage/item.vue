<template>
    <div class="itemPage mt-6">

        <div class="itemPage__main">
            <div class="d-flex justify-space-between documentTitle">
               <h1 class="pl-4"> {{getDocumentName}}</h1>
                 <shareContent v-if="getDocumentDetails" :link="shareContentParams.link"
              :twitter="shareContentParams.twitter"
              :whatsApp="shareContentParams.whatsApp"
              :email="shareContentParams.email"/>

            </div>

            <div class="itemPage__main__document">
                <mainItem :isLoad="isLoad" :document="document"></mainItem>

                <v-card class="itemActions pt-11 px-4 elevation-0">
                    <div class="wrapper d-flex justify-end pb-4">
                        <template v-if="getDocumentPrice && !getIsPurchased">
                            <div class="d-flex align-end mr-4">
                                <div class="mr-1 price">{{priceWithComma}}</div>
                                <span class="points" v-t="'documentPage_points'"></span>
                            </div>
                            <!-- <div v-t="'documentPage_credit_uploader'"></div> -->
                        </template>
                        <v-btn 
                            class="itemPage__side__btn white--text"
                            depressed
                            rounded
                            large
                            :loading="getBtnLoading"
                            @click="openPurchaseDialog"
                            v-if="!getIsPurchased"
                            height="42"
                            width="215"
                            color="#4c59ff">
                                <span v-if="isVideo" v-t="'documentPage_unlock_video_btn'"></span>
                                <span v-else v-t="'documentPage_unlock_document_btn'"></span>
                        </v-btn>
                        <v-btn
                            v-else
                            large
                            tag="a"
                            :href="`${$route.path}/download`"
                            target="_blank"
                            :loading="getBtnLoading"
                            class="itemPage__side__btn white--text"
                            width="215"
                            height="42"
                            depressed block rounded @click="downloadDoc" color="#4c59ff">
                            <span v-t="'documentPage_download_btn'"></span>
                        </v-btn>
                    </div>
                </v-card>

                <resultNote v-if="doucmentDetails.feedItem" class="itemPage__main__document__doc" :item="doucmentDetails.feedItem" :fromItemPage="true">
                    <!-- <template #arrowBack>
                        <v-icon
                            class="hidden-md-and-up document-header-large-sagment--arrow" 
                            @click="closeDocument" 
                            v-html="'sbf-arrow-left-carousel'">
                        </v-icon>
                    </template> -->
                </resultNote>

                <template v-else>
                    <v-sheet color="#fff" class="pb-2 skeletonWarp">
                        <v-skeleton-loader max-width="250" type="list-item-avatar-two-line"></v-skeleton-loader>
                        <v-skeleton-loader max-width="500" type="list-item-three-line, list-item"></v-skeleton-loader>
                    </v-sheet>
                </template>
            </div>

            <div v-if="itemList.length" class="itemPage__main__carousel" :class="{'itemPage__main__carousel--margin': !docTutor && !docTutor.isTutor && $vuetify.breakpoint.xsOnly}">
                <div class="itemPage__main__carousel__header">
                    <div class="itemPage__main__carousel__header__title" v-language:inner="'documentPage_related_content'"></div>
                    <router-link 
                        v-language:inner="'documentPage_full_list'"
                        :to="{name: 'feed', query: {Course: courseName}}"
                        class="itemPage__main__carousel__header--seeAll"
                        color="#4c59ff"
                    ></router-link>
                </div>
                <sbCarousel 
                    class="carouselDocPreview" 
                    @select="enterItemCard" 
                    :arrows="$vuetify.breakpoint.mdAndUp ? true : false"
                    :gap="20">
                        <itemCard class="itemCard-itemPage" :fromCarousel="true" v-for="(item, index) in itemList" :item="item" :key="index"/>
                </sbCarousel>
            </div>

            <div 
                class="itemPage__main__tutorCard"
                v-if="docTutor.isTutor"
                :class="{'itemPage__main__tutorCard--margin': docTutor.isTutor && $vuetify.breakpoint.xsOnly, 'itemPage__main__tutorCard--marginT': !itemList.length}"
            >
                <tutorResultCardMobile v-if="$vuetify.breakpoint.xsOnly" :tutorData="docTutor"></tutorResultCardMobile>
                <tutorResultCard v-else :tutorData="docTutor"></tutorResultCard>
            </div>
            <!-- <mobileUnlockDownload :sticky="true" v-if="$vuetify.breakpoint.md || $vuetify.breakpoint.sm" :document="document"></mobileUnlockDownload> -->
        </div>
        <!-- <mobileUnlockDownload v-if="$vuetify.breakpoint.xsOnly" :document="document"></mobileUnlockDownload> -->
        <unlockDialog :document="document"></unlockDialog>
        <v-snackbar
            v-model="snackbar"
            :top="true"
            :timeout="8000"
        >
            <div>
                <span v-language:inner="'resultNote_unsufficient_fund'"></span>
            </div>
            <v-btn
                class="px-4"
                outlined
                rounded
                @click="openBuyTokenDialog"
            >
                <span v-language:inner="'dashboardPage_my_sales_action_need_btn'"></span>
            </v-btn>
        </v-snackbar>
    </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';

//services
import * as dialogNames from '../global/dialogInjection/dialogNames.js';
import { LanguageService } from "../../../services/language/languageService";
import analyticsService from '../../../services/analytics.service';
import chatService from '../../../services/chatService';

//store
import storeService from '../../../services/store/storeService';
import document from '../../../store/document';
import studyDocumentsStore from '../../../store/studyDocuments_store';

// components
import mainItem from './components/mainItem/mainItem.vue';
import resultNote from '../../results/ResultNote.vue';
const sbCarousel = () => import(/* webpackChunkName: "sbCarousel" */'../../sbCarousel/sbCarousel.vue');
import itemCard from '../../carouselCards/itemCard.vue';
const tutorResultCard = () => import(/* webpackChunkName: "tutorResultCard" */ '../../results/tutorCards/tutorResultCard/tutorResultCard.vue');
const tutorResultCardMobile = () => import(/* webpackChunkName: "tutorResultCardMobile" */ '../../results/tutorCards/tutorResultCardMobile/tutorResultCardMobile.vue');
// import whyUsDesktop from './components/whyUs/whyUsDesktop.vue';
import whyUs from './components/whyUs/whyUs.vue';
// import mobileUnlockDownload from './components/mobileUnlockDownload/mobileUnlockDownload.vue';
import unlockDialog from './components/dialog/unlockDialog.vue';
const shareContent = () => import(/* webpackChunkName: "shareContent" */'../global/shareContent/shareContent.vue');
export default {
    name: 'itemPage',
    components: {
        resultNote,
        sbCarousel,
        tutorResultCard,
        tutorResultCardMobile,
        itemCard,
        // whyUsDesktop,
        whyUs,
        // mobileUnlockDownload,
        mainItem,
        unlockDialog,
        shareContent
    },
    props: {
        id: {
            type: String
        }
    },
    data() {
        return {
            docPage: 1,
            isLoad: false,
        }
    },
    watch:{
        '$route.params.id'(){
            this.clearDocument();
            this.documentRequest(this.id);        
            this.getStudyDocuments({course: this.$route.params.courseName , id: this.id})
        },
    },
    computed: {
        ...mapGetters([
            'getBannerParams',
            'accountUser',
            'getDocumentDetails',
            'getDocumentName',
            'getDocumentPrice',
            'getIsPurchased',
            'getRelatedDocuments',
            'getRouteStack',
            'getPurchaseConfirmation',
            'getShowItemToaster',
            'getBtnLoading',
        ]),
        shareContentParams(){
            let urlLink = `${global.location.origin}/d/${this.$route.params.id}?t=${Date.now()}` ;
            let itemType = this.getDocumentDetails.documentType;
            let courseName = this.courseName;
            let paramObJ = {
                link: urlLink,
                twitter: this.$t('shareContent_share_item_twitter',[courseName,urlLink]),
                whatsApp: this.$t('shareContent_share_item_whatsapp',[courseName,urlLink]),
                email: {
                    subject: this.$t('shareContent_share_item_email_subject',[courseName]),
                    body: this.$t('shareContent_share_item_email_body',[itemType,courseName,urlLink]),
                }
            }
            return paramObJ
        },
        snackbar: {
            get() {
                return this.getShowItemToaster
            },
            set(val) {
                this.updateItemToaster(val)
            }
        },
        document() {
            if(this.getDocumentDetails) {
                return this.getDocumentDetails;
            }
            return {}
        },

        doucmentDetails() {
            //TODO why explaind why not use document()
            if(this.getDocumentDetails && this.getDocumentDetails.details) {
                return this.getDocumentDetails.details
            }
            return {}
        },

        docTutor() {
            //TODO etc above
            if(this.getDocumentDetails && this.getDocumentDetails.details && this.getDocumentDetails.details.tutor) {
                return this.getDocumentDetails.details.tutor;
            }
            return {};
        },
        itemList() {
            return this.getRelatedDocuments;
        },
        firstName() {
            let user = this.docTutor;
            if(user.name) {
                return this.docTutor.name.split(' ')[0];
            }
            return ''
        },
        showFirstName() {
            let maxChar = 5;
            let user = this.docTutor;
            if(user.name) {
                let name = user.name.split(' ')[0];
                if(name.length > maxChar) {                    
                    return LanguageService.getValueByKey('resultTutor_message_me');
                }
                return name;
            }
            return null
        },
        courseName() {
            if(this.document && this.document.details) {
                return this.document.details.course;
            }
            return null
        },
        isMyProfile(){
            if(!!this.docTutor && !!this.accountUser){
                return (this.docTutor.isTutor && this.docTutor.userId == this.accountUser.id)
            }
            return false;
        },
        isVideo() {
            return this.document.documentType === 'Video';
        },
        priceWithComma() {
            if(this.document && this.document.details) {
                return this.document.details.price.toLocaleString();
            }
            return null
        },
    },
        methods: {
        ...mapActions([
            'documentRequest', 
            'clearDocument', 
            'getStudyDocuments', 
            'updateCurrTutor', 
            'setTutorRequestAnalyticsOpenedFrom', 
            'updateRequestDialog',
            'setActiveConversationObj',
            'openChatInterface',
            'updateItemToaster',
            'updatePurchaseConfirmation',
        ]),
        
        enterItemCard(vueElm){
            //TODO DUplicate code
            if(vueElm.enterItemPage){
                vueElm.enterItemPage();
            }else{
                vueElm.$parent.enterItemPage();
            }
            this.isLoad = true;
            setTimeout(()=>{
                this.isLoad = false;
            })
            this.$nextTick(() => {
                this.documentRequest(this.id);
            })
        },
        closeDocument() {
            let regRoute = 'registration';
            let routeStackLength = this.getRouteStack.length;
            let beforeLastRoute = this.getRouteStack[routeStackLength-2];
            if (routeStackLength > 1 && beforeLastRoute && beforeLastRoute.name !== regRoute && beforeLastRoute.name !== 'document') {
                this.$router.back();
            } else {
                this.$router.push({ name: "feed" });
            }
        },
        sendMessage() {
            let user = this.docTutor;
            if (this.accountUser == null) {
                analyticsService.sb_unitedEvent('Tutor_Engagement', 'contact_BTN_item_page', `userId:GUEST`);
                this.updateCurrTutor(user);
                this.setTutorRequestAnalyticsOpenedFrom({
                    component: 'tutorCard',
                    path: this.$route.path
                });
                this.updateRequestDialog(true);
            } 
            else {
                analyticsService.sb_unitedEvent('Tutor_Engagement', 'contact_BTN_profile_page', `userId:${this.accountUser.id}`);
                let conversationObj = {
                    userId: user.userId,
                    image: user.image,
                    name: user.name,
                    conversationId: chatService.createConversationId([user.userId, this.accountUser.id]),
                }
                let currentConversationObj = chatService.createActiveConversationObj(conversationObj)
                this.setActiveConversationObj(currentConversationObj);
                
                this.openChatInterface();                    
            }
        },
        moveDownToTutorItem() {
            let elem = this.$el.querySelector('.itemPage__main__tutorCard');
            elem.scrollIntoView({ behavior: 'smooth', block: 'center' })
        },
        openBuyTokenDialog() {
            this.updateItemToaster(false);
            this.$openDialog(dialogNames.BuyPoints);
        },
        openPurchaseDialog() {
            if(this.accountUser) {
                this.updatePurchaseConfirmation(true)
            } else {
                this.$store.commit('setComponent', 'register')
            }
        },
    },
    beforeDestroy(){
        this.clearDocument();
        //TODO missing unresister
        storeService.unregisterModule(this.$store,'document');
    },
    mounted(){
        this.documentRequest(this.id);        
        this.getStudyDocuments({course: this.$route.params.courseName , id: this.id})
    },
    created() {    
        storeService.lazyRegisterModule(this.$store,'studyDocumentsStore',studyDocumentsStore); 
        storeService.registerModule(this.$store,'document', document);
        
    }
}
</script>

<style lang="less">
    @import '../../../styles/mixin';

    .itemPage {
        .documentTitle {
            background: #fff;
            & > div {
                .flexSameSize();
              
            }
            & > h1 {
                .flexSameSize();
                  font-size: 18px;
                font-weight: 600;
                color: #43425d;
                align-self: center;
            }
        }
        position: relative;
        margin: 0 auto;
        max-width: 960px;
        @media (max-width: @screen-md) {
            margin: 20px;
        }
        @media (max-width: @screen-xs) {
            margin: 0;
            display: block;
        }
        .sticky-item{
            position: sticky;
            height: fit-content;
            top: 80px;
            &.sticky-item_bannerActive{
                top: 150px;
            }
        }
        &__main {
            @media (max-width: @screen-sm) {
                margin-right: 0;
                max-width: auto;
            }
            &__document {
                width: 100%;
                margin: 0 auto 16px;

                @media (max-width: @screen-sm) {
                    width: auto;
                }
                .itemActions {
                    color: @global-purple;
                    .wrapper {
                        border-bottom: 1px solid #ddd;
                        font-weight: 600;
                        .price {
                            .responsive-property(font-size, 30px, null, 64px);                
                        }
                        .points {
                            .responsive-property(font-size, 14px, null, 64px);
                        }
                    }
                }
                &__doc {
                    padding: 12px 16px 12px 12px;
                }
                .document-header-large-sagment {
                    &--arrow {
                        transform: none /*rtl:scaleX(-1)*/;
                        margin-right: 18px;
                        font-size: 20px;
                    }
                }
                .skeletonWarp {
                    .v-skeleton-loader__avatar {
                        border-radius: 50%;
                    }
                }
                &__tutor {
                    display: flex;
                    align-items: center;
                    flex-wrap: wrap;
                    font-weight: 600;
                    font-size: 14px;
                    &__link {
                        @media (max-width: @screen-md) {
                            margin-bottom: 6px;
                        }
                        &--title1 {
                            display: inline-block;
                            color: #5560ff;
                            cursor: pointer;
                            @media (max-width: @screen-xs) {
                                white-space: nowrap;
                                display: block;
                            }
                        }
                        &--title2 {
                            color: #4d4b69;
                            display: inline-block;
                            cursor: text;
                            @media (max-width: @screen-xs) {
                                white-space: nowrap;
                                display: block;
                            }
                        }
                        @media (max-width: @screen-xs) {
                            flex-direction: column;
                            justify-content: center;
                            margin-bottom: 10px;
                        }
                    }
                    &--btn {
                        border: solid 1px #4452fc;
                        border-radius: 28px;
                        background: #fff !important; //vuetify
                        @media (max-width: @screen-xs) {
                            padding: 0 10px
                        }
                        div {
                            color: #4452fc;
                            font-size: 13px;
                            font-weight: 600;
                            text-transform: initial;
                            margin-bottom: 1px;
                        }
                    }
                }
                &--loader {
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    min-height: 160px;
                }
            }
            &__carousel {
                margin: 38px 0 34px 0;

                @media (max-width: @screen-xs) {
                    background: #fff;
                    padding: 16px 11px;
                    margin: 0 0 16px 0;
                }
                &__header {
                    display: flex;
                    justify-content: space-between;
                    align-items: center;
                    margin-bottom: 14px;
                    font-weight: 600;
                    &__title{
                        color: #43425d;
                        font-size: 18px;
                        font-weight: 700;                       
                    }
                    &--seeAll {
                        color: #4c59ff !important;
                        font-size: 14px;
                    }
                }
                &--margin {
                    margin-bottom: 100px;
                }
                .carouselDocPreview {
                    .itemCard-itemPage {
                        .item-cont {
                            z-index: 3 !important; //flicking
                            @media (max-width: @screen-xs) {
                                overflow: visible !important; //flicking
                            }
                        }
                    }
                    .sbCarousel_btn {
                        i {
                            font-size: 18px;
                        }
                    }
                }
            }
            &__tutorCard {
                &--marginT {
                    margin-top: 34px;
                }
                &--margin {
                    margin-bottom: 100px;
                }
                &--marginTop {
                    margin-top: 34px; 
                }
            }
        }
    }
</style>