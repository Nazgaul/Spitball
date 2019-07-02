<template>
    <div class="main-container">
        <!-- {{this.getFetch}} -->
        <v-layout row wrap class="main-header pb-3" align-center>
            <v-icon color="#000" class="display-2" @click="closeDocument">sbf-arrow-back-chat</v-icon>
            <v-icon>FileType</v-icon>
            <span class="pl-3 headline courseName font-weight-bold">{{courseName}}</span>
            <v-spacer></v-spacer>
            <span class="pr-5 grey-text"><v-icon class="pr-2" small>sbf-views</v-icon>{{docViews}}</span>
            <span class="pr-4 grey-text">{{documentDate}}</span>
            <v-btn
                :depressed="true"
                slot="activator"
                icon>
                  <v-icon class="verticalMenu">sbf-3-dot</v-icon>
            </v-btn>
        </v-layout>
        <div class="page">
            <div class=" text-xs-center"  v-for="(page, index) in docPreview" :key="index">
                <component 
                    class="page-content" 
                    :is="currentComponent" 
                    :src="page"
                    :alt="document.content">
                </component>
            </div>
            <div class="unlockBox headline" v-if="isFetching" @click="unlockDocument">
                <p class="text-xs-left" v-language:inner="'documentPage_unlock_document'"></p>
                <div class="aside-top-btn elevation-5">
                    <span class="pa-4 font-weight-bold text-xs-center">12.00 Pt</span>
                    <span class="white--text pa-4 font-weight-bold text-xs-center" v-language:inner="'documentPage_unlock_btn'"></span>
                </div>
            </div>
        </div>

    </div>
</template>
<script>
import { mapActions, mapGetters, mapMutations } from 'vuex';

export default {
    name: 'mainDocument',
    props: {
        document: {
            type: Object
        },
    },
    data() {
        return {
        }
    },
    methods: {
        ...mapActions(['clearDocument', 'purchaseDocument']),
        ...mapMutations(['setFetch']),

        unlockDocument() {
            let item = {id: this.document.details.id, price: this.document.details.price}
            this.purchaseDocument(item);
        },
        closeDocument() {
            this.clearDocument();
            this.$router.go(-1);
        }
    },
    computed: {
        ...mapGetters(['getFetch']),

        currentComponent() {
            if (this.document && this.document.contentType) {
                return this.document.contentType === "html" ? "iframe" : "img";
                if (['link', 'text'].find((x) => x === type.toLowerCase())) return 'iframe'
            }
        },
        courseName() {
            if(this.document.details && this.document.details.course) {
                return this.document.details.course
            }
        },
        documentDate() {
            if(this.document.details && this.document.details.date) {
                return new Date(this.document.details.date).toLocaleString('en-US', {year: 'numeric', month: 'short', day: 'numeric'})
            }
        },
        isPurchased() {
            if(this.document.details && this.document.details.isPurchased) {
                return this.document.details.isPurchased;
            }
        },
        docViews() {
            if(this.document.details && this.document.details.views) {
                return this.document.details.views
            }
        },
        docPreview() {
            if(this.document.preview) {
                return this.document.preview
            }
        },
        isFetching() {
            if(this.isPurchased && !this.getFetch) {
                return true;
            }
            return false;
        }
    }
}
</script>
<style lang="less">
    .main-container {
        flex: 5;
        .main-header {
            .grey-text {
                opacity: .6;
            }
            .verticalMenu {
                color: #aaa;
            }
        }
        .page {
            position: relative;
            .unlockBox {
                cursor: pointer;
                background: #fff;
                position: fixed;
                border: 2px solid #000;
                padding: 20px;
                left: 0;
                top: 200px;
                right: 330px;
                bottom: 0;
                height: 200px;
                width: 550px;
                margin: auto;
                p {
                    width: 60%;
                }
                .aside-top-btn {
                    display: flex;
                    border-radius: 4px;
                    margin: 0 0 0 auto;
                    width: 60%;
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
            }
            .page-content {
                width: 100%;
            }
        }
        
    }
</style>