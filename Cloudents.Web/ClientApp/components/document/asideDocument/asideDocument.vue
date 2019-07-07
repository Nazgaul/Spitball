<template>
    <div class="aside-container">
        
        <div class="aside-top mb-2" :class="[$vuetify.breakpoint.smAndDown ? 'pa-2' : 'pa-3']">
            <v-layout justify-space-between>
                <v-icon color="#43425d">sbf-spitball</v-icon>
                <v-icon class="hidden-md-and-up subheading" @click="closeDocument">sbf-close</v-icon>
            </v-layout>

            <p class="pt-3 font-weight-bold" v-language:inner="'documentPage_student_learn'"></p>
            <p class="caption" v-language:inner="'documentPage_online_tutor'"></p>

            <my-courses class="d-block mx-auto hidden-sm-and-down"></my-courses>

            <p class="caption font-weight-black pt-2 text-xs-center hidden-sm-and-down" v-language:inner="'documentPage_credit_uploader'"></p>

            <div class="aside-top-btn btn-lock elevation-5" v-if="!isPurchased" @click="unlockDocument">
                <span class="pa-4 font-weight-bold text-xs-center">12.00 Pt</span>
                <span class="white--text pa-4 font-weight-bold text-xs-center" v-language:inner="'documentPage_unlock_btn'"></span>
            </div>

            <div class="aside-top-btn btn-download elevation-5 justify-center" v-if="isPurchased" @click="downloadDoc">                    
                <v-icon color="#fff" class="pr-3">sbf-download-cloud</v-icon>
                <span class="white--text py-4 font-weight-bold" v-language:inner="'documentPage_download_btn'"></span>
            </div>
            
            <table class="py-3">
                <tr v-if="isCourse">
                    <td class="py-2" v-language:inner="'documentPage_table_course'"></td>
                    <td class="caption"><router-link :to="{path: '/ask', query: {Course: getCourse} }">{{getCourse}}</router-link></td>
                </tr>
                <tr v-if="isUniversity">
                    <td class="py-2" v-language:inner="'documentPage_table_university'"></td>
                    <td class="caption">{{getUniversity}}</td>
                </tr>
                <tr v-if="isType">
                    <td class="py-2" v-language:inner="'documentPage_table_type'"></td>
                    <td class="caption">{{getType}}</td>
                </tr>
            </table>
        </div>
        
        <div class="aside-cards mb-5">
            <student-card :tutorData="ownTutor" v-if="!isTutor && ownTutor"/>
            <tutor-result-card-mobile :tutorData="ownTutor" :singleCard="true" :isInTutorList="true" v-if="isTutor && ownTutor" />
        </div>

        <aside-document-tutors v-if="!$vuetify.breakpoint.smAndDown"/>

    </div>
</template>
<script>
import { mapActions } from 'vuex';

import tutorResultCardMobile from '../../../components/results/tutorCards/tutorResultCardMobile/tutorResultCardMobile.vue';
import asideDocumentTutors from './asideDocumentTutors.vue';
import studentCard from '../studentCard.vue';

import myCourses from '../../../font-icon/my-courses-image.svg';

export default {
    components: {
        myCourses,
        studentCard,
        tutorResultCardMobile,
        asideDocumentTutors
    },
    props: {
        document: {},
    },
    methods: {
        ...mapActions(['purchaseDocument', 'downloadDocument', 'clearDocument']),

        unlockDocument() {
            let item = {id: this.document.details.id, price: this.document.details.price}
            this.purchaseDocument(item);
        },
        downloadDoc() {
            let item = {
                url: `${this.$route.path}/download`,
                course: this.document.details.course,
                id: this.document.details.id
            }
            this.downloadDocument(item)
        },
        closeDocument() {
            this.clearDocument();
            this.$router.go(-1);
        },
    },
    computed: {
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
        isPurchased() {
            if(this.document.details && this.document.details.isPurchased) {
                return this.document.details.isPurchased;
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
                return this.document.details.user.price
            }
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
                color: #43425d;
            }
            p:nth-child(5) {
                color: #4452fc;
            }
            .aside-top-btn {
                cursor: pointer;
                display: flex;
                border-radius: 4px;
                &.btn-lock {
                    @media (max-width: @screen-sm) {
                        background: #fff;
                    }
                    span:first-child {
                        flex: 2;
                        font-size: 18px;
                    }
                    span:nth-child(2) {
                        flex: 1;
                        background-color: #4452fc;
                        font-size: 15px;
                        border-radius: 0 4px 4px 0
                    }
                }
                &.btn-download {
                    background-color: #4452fc;
                    .download {
                        font-size: 15px;
                        border-radius: 0 4px 4px 0
                    }
                }
                @media (max-width: @screen-sm) {
                    margin: 10px;
                    position: fixed;
                    bottom: 0;
                    left: 0;
                    right: 0;
                    z-index: 9999;
                }
            }
            table {
                width: 100%;
                td:first-child {
                    color: #aaa;
                }
                td:nth-child(2) {
                    border-bottom: 2px solid #ccc;
                }
                tr:last-child {
                    td:last-child {
                        border-bottom: none;
                    }
                } 
            }
        }
    }
</style>