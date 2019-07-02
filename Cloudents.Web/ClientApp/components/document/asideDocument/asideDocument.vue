<template>
    <div class="aside-container">
        
        <div class="aside-top pa-3 mb-2">
            <v-icon color="#43425d">sbf-spitball</v-icon>
            <p class="pt-3 font-weight-bold" v-language:inner="'documentPage_student_learn'"></p>
            <p class="caption" v-language:inner="'documentPage_online_tutor'"></p>
            <my-courses class="d-block mx-auto"></my-courses>
            <p class="caption font-weight-black pt-2 text-xs-center" v-language:inner="'documentPage_credit_uploader'"></p>
            <div class="aside-top-btn-lock elevation-5" v-if="isFetching" @click="unlockDocument">
                <span class="pa-4 font-weight-bold text-xs-center">12.00 Pt</span>
                <span class="white--text pa-4 font-weight-bold text-xs-center" v-language:inner="'documentPage_unlock_btn'"></span>
            </div>
            <div class="aside-top-btn-download elevation-5 justify-center" v-if="!isFetching" @click="download">                    
                <v-icon color="#fff" class="pr-3">sbf-download-cloud</v-icon><span class="white--text py-4 font-weight-bold" v-language:inner="'documentPage_download_btn'">Download Document</span>
            </div>
            <table class="pt-3">
                <tr>
                    <td class="py-2" v-language:inner="'documentPage_table_course'"></td>
                    <td class="caption"><router-link :to="{path: '/ask', query: {Course: getCourse} }">{{getCourse}}</router-link></td>
                </tr>
                <tr>
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
            <tutor-result-card-mobile :tutorData="document" :singleCard="true" :isInTutorList="true" />
        </div>

        <v-layout justify-space-between class="pb-3">
            <span class="font-weight-bold more-tutors" v-language:inner="'documentPage_more_tutors'"></span>
            <router-link v-language:inner="'documentPage_see_all'" to="/tutor"></router-link>
        </v-layout>

        <div v-for="(tutor, index) in tutorList" :key="index">
            <tutor-result-card-mobile :tutorData="tutor" :isInTutorList="true" />
        </div>

    </div>
</template>
<script>
import myCourses from '../../../font-icon/my-courses-image.svg';
import { mapGetters, mapActions, mapMutations } from 'vuex';

import tutorResultCardMobile from '../../../components/results/tutorCards/tutorResultCardMobile/tutorResultCardMobile.vue';

export default {
    components: {
        myCourses,
        tutorResultCardMobile
    },
    props: {
        document: {},
    },
    data() {
        return {

        }
    },
    methods: {
        ...mapActions(['getTutorListCourse', 'purchaseDocument', 'downloadDocument']),
        ...mapMutations(['setFetch']),

        unlockDocument() {
            let item = {id: this.document.details.id, price: this.document.details.price}
            this.purchaseDocument(item);
        },
        download() {
            let item = {
                url: `${this.$route.path}/download`,
                course: this.document.details.course,
                id: this.document.details.id
            }
            this.downloadDocument(item)
        }
    },
    computed: {
        ...mapGetters(['getTutorList', 'getFetch']),

        isPurchased() {
            if(this.document.details && this.document.details.isPurchased) {
                return this.document.details.isPurchased;
            }
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
        isType() {
            return this.document.details && this.document.details.type;
        },
        tutorList() {
            return this.getTutorList
        },
        isFetching() {
            if(this.isPurchased && !this.getFetch) {
                return true;
            }
            return false;
        }
    },
    created() {      
        let course = this.$route.params.courseName
        this.getTutorListCourse(course)
    }
}
</script>
<style lang="less">
    .aside-container {
        flex: 1;
        .aside-top {
            border-radius: 4px;
            background-color: #ffffff;
            p:nth-child(2) {
                font-size: 15px;
                color: #43425d;
            }
            p:nth-child(5) {
                color: #4452fc;
            }
            .aside-top-btn-lock {
                cursor: pointer;
                display: flex;
                border-radius: 4px;
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
            .aside-top-btn-download {
                cursor: pointer;
                display: flex;
                border-radius: 4px;
                background-color: #4452fc;
                .download {
                    font-size: 15px;
                    border-radius: 0 4px 4px 0
                }
            }
            table {
                width: 100%;
                td:first-child {
                    color: #bbb;
                }
                td:nth-child(2) {
                    border-bottom: 2px solid #eee;
                }
                tr:last-child {
                    td:last-child {
                        border-bottom: none;
                    }
                } 
            }
        }
        .more-tutors {
            font-size: 15px;
        }
    }
</style>