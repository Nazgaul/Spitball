<template>
    <div class="mobileUnlockDownload" :class="{'mobileUnlockDownload--sticky': sticky, 'mobileUnlockDownload--purchased': isPurchased}" v-if="showBlock && !isLoading">
        <div class="mobileUnlockDownload__title" v-language:inner="'documentPage_credit_uploader'" v-if="!zeroPrice && !isPurchased"></div>
        <div class="mobileUnlockDownload__action">
            <template v-if="!zeroPrice && !btnLoader && !isPurchased">
                <span class="mobileUnlockDownload__action__price">{{priceWithComma}}</span>
                <span class="mobileUnlockDownload__action__pts" v-language:inner="'documentPage_points'"></span>
            </template>
            <v-btn
                class="mobileUnlockDownload__action__btn"
                depressed
                block
                large
                rounded
                :loading="isLoading"
                @click="openPurchaseDialog"
                v-if="!isPurchased || isVideo"
                color="#4c59ff">
                    <span v-if="isVideo" v-language:inner="'documentPage_unlock_video_btn'"></span>
                    <span v-else v-language:inner="'documentPage_unlock_document_btn'"></span>
            </v-btn>
            <v-btn
                v-else
                tag="a"
                large
                :href="`${$route.path}/download`"
                target="_blank"
                class="mobileUnlockDownload__action__btn"
                depressed
                :loading="isLoading"
                block
                rounded
                @click="downloadDoc" color="#4c59ff">
                    <span v-language:inner="'documentPage_download_btn'"></span>
            </v-btn>
        </div>
    </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';

export default {
    name: 'mobileUnlockDownload',
    props: {
        document: {
            type: Object
        },
        sticky: {
            type: Boolean
        }
    },
    data() {
        return {
            btnLoader: true,
        }
    },
    computed: {
        ...mapGetters(['accountUser']),

        showBlock() {
            if(this.isVideo && !this.isShowPurchased) {
                return true;
            } else if (this.isVideo && this.isPurchased) {
                return false
            }
            return true;
        },
        zeroPrice() {
            return (this.document && this.document.details && this.document.details.price === 0);
        },
        priceWithComma() {
            if(this.document && this.document.details) {
                return this.document.details.price.toLocaleString();
            }
        },
        docPrice() {
            if(this.document && this.document.details) {
                return this.document.details.price;
            }
            return 0;
        },
        isPrice() {
            if (this.document.details && this.document.details.price > 0) {
                return true;
            } else {
                return false;
            }
        },
        isPurchased() {
            if (this.document.details) {
                return this.document.details.isPurchased;
            }
            return false;
        },
        isShowPurchased() {
            if (this.isPurchased && this.docPrice === 0) {
                return true;
            }
            return false;
        },
        isLoading() {
            if(this.document && this.document.details && !this.btnLoader) {
                return false;
            }
            return true;
        },
        isVideo() {
            return this.document.documentType === 'Video';
        },
    },
    methods: {
        ...mapActions(['downloadDocument', 'updatePurchaseConfirmation', 'updateLoginDialogState']),

        openPurchaseDialog() {
            if(this.accountUser) {
                this.updatePurchaseConfirmation(true)
            } else {
                this.updateLoginDialogState(true);
            }
        },
        downloadDoc(e) {
            if (!this.accountUser) {
                e.preventDefault();
            }
            let item = {
                course: this.document.details.course,
                id: this.document.details.id
            };
            this.downloadDocument(item);
        },
    },
    created() {
        this.btnLoader = false;
    }
}
</script>

<style lang="less">
@import '../../../../../styles/mixin.less';
.mobileUnlockDownload{
    background: white;
    width: 100%;
    box-shadow: 0 0 10px 1px rgba(0, 0, 0, 0.22);
    color: #4d4b69;
    position: fixed;
    bottom: 0;
    left: 0;
    right: 0;
    padding: 8px 0;
    text-align: center;
    z-index: 4;
    margin-top: 20px;
    
    &--sticky {
        position: sticky;
        bottom: 0;
    }
    &--purchased {
        height: auto;

        .mobileUnlockDownload__action {
            padding: 6px;
        }
    }
    &__title{
        font-size: 14px;
        font-weight: 600;
        font-stretch: normal;
        font-style: normal;
        line-height: normal;
        letter-spacing: normal;
    }
    &__action{
        max-width: 312px;
        margin: 0 auto;
        padding-top: 10px;
        &__price{
            font-size: 22px;
            font-weight: 700;
            margin-right: 2px;
        }
        &__pts{
            font-size: 14px;
            font-weight: 600;
            margin-right: 10px;  
        }
        &__btn{
            color: white !important;
            height: 40px !important;
            min-width: 204px;
            max-width: 216px;  
            text-transform: capitalize  !important;
            font-size: 18px;
            font-weight: 600;
            vertical-align: bottom;
            margin: 0;
            display: inline-flex;
        }
    }
}
</style>
