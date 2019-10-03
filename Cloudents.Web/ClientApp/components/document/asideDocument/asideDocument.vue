<template>
    <div class="aside-container">
        
        <div class="aside-top mb-3" :class="[$vuetify.breakpoint.smAndDown ? 'pa-2' : 'pa-3']">
            <v-layout justify-space-between>
                <v-icon @click="goToNote" color="#43425d">sbf-spitball</v-icon>
                <v-icon class="hidden-md-and-up subheading" @click="closeDocument">sbf-close</v-icon>
            </v-layout>

            <p class="pt-3 font-weight-bold subheading" v-language:inner="'documentPage_student_learn'"></p>
            <p class="body-1 get-online" v-language:inner="'documentPage_online_tutor'"></p>

            <my-courses class="d-block mx-auto hidden-sm-and-down"></my-courses>

            <p class="caption font-weight-bold pt-2 text-xs-center hidden-sm-and-down" v-if="isShowPurchased" v-language:inner="'documentPage_credit_uploader'"></p>

            <template v-if="$vuetify.breakpoint.smAndDown && !isVideo">
                <div class="aside-top-btn btn-lock" v-if="isShowPurchased && !isLoading" @click="accountUser? updatePurchaseConfirmation(true) : updateLoginDialogState(true)">
                    <span class="pa-4 font-weight-bold text-xs-center" v-if="isPrice">{{docPrice | currencyLocalyFilter}}</span>
                    <span class="white--text pa-4 font-weight-bold text-xs-center body-1" v-language:inner="'documentPage_unlock_btn'"></span>
                </div>
                <a class="aside-top-btn index btn-download justify-center" @click="downloadDoc" :href="`${$route.path}/download`" target="_blank" :class="{'mt-2': !isShowPurchased}" v-if="!isShowPurchased && !isLoading">                    
                    <v-icon color="#fff" class="pr-3">sbf-download-cloud</v-icon>
                    <span class="white--text font-weight-bold" v-language:inner="'documentPage_download_btn'"></span>
                </a>
            </template>

            <v-progress-circular
                class="unlock_progress"
                v-if="isLoading && !isPurchased"
                indeterminate
                color="#4452fc"
            ></v-progress-circular>
        </div>
        <div class="aside-top">
            <table class="pa-2 pb-2">
                <tr v-if="isName">
                    <td class="py-2 font-weight-bold text-truncate" v-language:inner="'documentPage_table_uploaded'"></td>
                    <td class=""><h3 class="body-1 text-truncate align-switch-r"><router-link :to="{path: '/profile', name: 'profile', params: {id: getUserId, name: getUploaderName} }">{{getUploaderName}}</router-link></h3></td>
                </tr>
                <tr v-if="isCourse">
                    <td class="py-2 font-weight-bold text-truncate" v-language:inner="'documentPage_table_course'"></td>
                    <td class=""><h3 class="body-1 text-truncate align-switch-r"><router-link :to="{path: '/note', query: {Course: getCourse} }">{{getCourse}}</router-link></h3></td>
                </tr>
                <tr v-if="isUniversity">
                    <td class="py-2 font-weight-bold text-truncate" v-language:inner="'documentPage_table_university'"></td>
                    <td class=""><h3 class="body-1 text-truncate align-switch-r">{{getUniversity}}</h3></td>
                </tr>
                <tr v-if="isType">
                    <td class="py-2 font-weight-bold text-truncate" v-language:inner="'documentPage_table_type'"></td>
                    <td class=""><h3 class="body-1 text-truncate align-switch-r">{{getType}}</h3></td>
                </tr>
            </table>
            <tutor-result-card-other :tutorData="ownTutor" :uploader="true"  v-if="isTutor && ownTutor" />
        </div>

        <aside-document-tutors :courseName="getCourse" v-if="!$vuetify.breakpoint.smAndDown && getCourse"/>
        <v-flex class="footer-holder text-xs-center mb-5" v-if="!$vuetify.breakpoint.smAndDown && getCourse">
            <router-link :to="{name: 'tutors',query:{Course:getCourse}}" class="subheading font-weight-bold tutors-footer" v-language:inner="'documentPage_full_list'"></router-link>
        </v-flex>
    </div>
</template>
<script>
import { mapActions, mapGetters } from 'vuex';
import asideDocumentTutors from './asideDocumentTutors.vue';
import tutorResultCardMobile from '../../../components/results/tutorCards/tutorResultCardMobile/tutorResultCardMobile.vue';
import tutorResultCardOther from '../../../components/results/tutorCards/tutorResultCardOther/tutorResultCardOther.vue';
import myCourses from '../../../font-icon/my-courses-image.svg';

export default {
    components: {
        myCourses,
        tutorResultCardMobile,
        tutorResultCardOther,
        asideDocumentTutors
    },
    props: {
        document: {},
    },
    methods: {
        ...mapActions(['downloadDocument', 'updatePurchaseConfirmation','updateLoginDialogState']),
        downloadDoc(e) {
            if(!this.accountUser){
                e.preventDefault();
            }
            let item = {
                url: `${this.$route.path}/download`,
                course: this.document.details.course,
                id: this.document.details.id
            }
            this.downloadDocument(item)
        },
        closeDocument() {
            let routeStackLength = this.getRouteStack.length;
            if(routeStackLength > 1){
                this.$router.back();
            }else{
                this.$router.push({path: '/note'})
            }
        },
        goToNote(){
            this.$router.push({path: '/note'});
        },
    },
    computed: {
        ...mapGetters(['getBtnLoading', 'accountUser', 'getRouteStack']),
        isVideo(){      
            return this.document.documentType === 'Video' 
        },
        getCourse() {
            if(this.document.details && this.document.details.course) {
                return this.document.details.course;
            }
        },
        getUniversity() {
            if(this.document.details && this.document.details.university) {
                return this.document.details.university;
            }
        },
        getType() {
            if(this.document.details && this.document.details.type) {
                return this.document.details.type;
            }
        },
        getUploaderName() {
            if(this.document.details && this.document.details.user) {
                return this.document.details.user.name;
            }
        },
        getUserId() {
            if(this.document.details && this.document.details.user) {
                return this.document.details.user.userId || this.document.details.user.id;
            }
        },
        isPurchased() {
            if(this.document.details && this.document.details.isPurchased) {
                return this.document.details.isPurchased;
            } else {
                return false;
            }
        },
        isType() {
            return this.document.details && this.document.details.type;
        },
        isUniversity() {
            return this.document.details && this.document.details.university;
        },
        isCourse() {
            return this.document.details && this.document.details.course;
        },
        ownTutor() {
            if(this.document.details && this.document.details.user) {
                return this.document.details.user;
            }
        },
        isTutor() {
            if(this.document.details && this.document.details.user) {
                return this.document.details.user.isTutor
            }
        },
        isLoading() {
            return this.getBtnLoading
        },
        docPrice() {
            if(this.document.details && this.document.details.price >= 0) {
                return this.document.details.price.toFixed(2)
            }
        },
        isPrice() {
            if(this.document.details && this.document.details.price > 0) {
                return true
            } 
            return false
            
        },
        isShowPurchased() {
            if(!this.isPurchased && this.isPrice > 0) {
                return true
            }
            return false
        },
        isName() {
            return (this.document.details && this.document.details.user) ? true : false;
        }
    }
}
</script>
<style lang="less">
    @import "../../../styles/mixin.less";

    .aside-container {
        flex: 1;
        .aside-top {
            border-radius: 4px;
            background-color: #ffffff;
            @media (max-width: @screen-sm) {
                background: inherit;
            }
            p:nth-child(2) {
                font-size: 15px;
                color: @global-purple;
            }
            p:nth-child(3) {
                color: @global-purple;
            }
            p:nth-child(5) {
                color: @global-blue;
            }
            .get-online{
                margin-bottom: 10px;
                line-height: 21px;
            }
            .index{
                z-index: 0 !important;
            }
            .aside-top-btn {
                cursor: pointer;
                display: flex;
                border-radius: 4px;
                margin: 0 20px;
                font-size: 15px;
                @media(max-width: @screen-sm){
                    border-radius: 0;
                    margin: unset;
                }
                &.btn-lock {
                    display: flex;
                    border: 1px solid #ccc;
                    border-radius: 4px;
                    margin: 0 auto;
                    line-height: 20px;
                    font-size: 15px;
                    z-index: 1;
                    @media (max-width: @screen-sm) {
                        background: #fff;
                        border-radius: 0;
                        margin: unset;
                    }
                    span:first-child {
                        flex: 2;
                        align-self: center;
                    }
                    span:nth-child(2) {
                        flex: 1;
                        background-color: @global-blue;
                        border-radius: 0 4px 4px 0
                    }
                }
                &.btn-download {
                    height: 70px;
                    background-color: @global-blue;
                    i {
                        font-size: 26px;     
                    }
                    span {
                        display: flex;
                        align-items: center;
                        font-size: 20px;
                    }
                }
                @media (max-width: @screen-sm) {
                    position: fixed;
                    bottom: 0;
                    left: 0;
                    right: 0;
                    z-index: 9999;
                }
            }
            table {
                width: 100%;
                td {
                    max-width: 0;
                }
                td:first-child {
                    color: #42415c;
                }
                td {
                    border-bottom: solid 1px rgba(67, 66, 93, 0.17);
                    h3 {
                        color: #5158af;
                        a {
                            color: #5158af;
                        }
                        font-weight: bold;
                        @media(max-width: @screen-sm){
                            font-weight: normal;
                        }
                    }
                }
                tr:last-child {
                    td {
                        border-bottom: none;
                    }
                } 
            }
            .unlock_progress {
                display: flex;
                margin: 0 auto;
            }
        }
        .footer-holder {
            a {
                color: #4452fc;
            }
        }
        .align-switch-r {
            text-align: right !important;
        }
    }
</style>