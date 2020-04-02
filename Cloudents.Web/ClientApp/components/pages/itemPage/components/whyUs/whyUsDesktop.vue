<template>
    <div v-if="showBlock" class="itemPage__side">
        <template v-if="!isLoading">
            <template v-if="!zeroPrice && !isPurchased">
                <div class="itemPage__side__top">
                    <div class="itemPage__side__top__price">{{priceWithComma}}</div>
                    <span class="itemPage__side__top__pts" v-language:inner="'documentPage_points'"></span>
                </div>
                <div class="itemPage__side__credit" v-language:inner="'documentPage_credit_uploader'"></div>
            </template>
            <v-btn 
                class="itemPage__side__btn white--text"
                depressed
                block
                rounded
                large
                :loading="isLoading"
                @click="openPurchaseDialog"
                v-if="!isPurchased || isVideo"
                color="#4c59ff">
                    <span v-if="isVideo" v-language:inner="'documentPage_unlock_video_btn'"></span>
                    <span v-else v-language:inner="'documentPage_unlock_document_btn'"></span>
            </v-btn>
            <v-btn
                v-else
                large
                tag="a"
                :href="`${$route.path}/download`"
                target="_blank"
                :loading="isLoading"
                class="itemPage__side__btn white--text"
                depressed block rounded @click="downloadDoc" color="#4c59ff">
                <span v-language:inner="'documentPage_download_btn'"></span>
            </v-btn>
            <div class="itemPage__side__bottom">
                <div class="itemPage__side__bottom__cont mb-3">
                    <shield class="itemPage__side__bottom__cont__icon"></shield>
                    <span class="itemPage__side__bottom__cont__span" v-language:inner="'documentPage_money_back'"></span>
                </div>
                <div class="itemPage__side__bottom__cont mb-3">
                    <secure class="itemPage__side__bottom__cont__icon"></secure>
                    <span class="itemPage__side__bottom__cont__span" v-language:inner="'documentPage_secure_payment'"></span>
                </div>
                <div class="itemPage__side__bottom__cont mb-3">
                    <exams class="itemPage__side__bottom__cont__icon"></exams>
                    <span class="itemPage__side__bottom__cont__span" v-language:inner="'documentPage_prepared_exams'"></span>
                </div>
            </div>
        </template>

        <template v-else>
            <v-sheet color="#fff" class="skeletonWrap">
                <v-skeleton-loader class="ml-auto mb-2" type="text" max-width="40"></v-skeleton-loader>
                <v-skeleton-loader class="mx-auto mb-2" type="text" max-width="200"></v-skeleton-loader>
                <v-skeleton-loader class="skeletonButton mb-4" type="button"></v-skeleton-loader>
                <v-skeleton-loader class="mb-2" type="text"></v-skeleton-loader>
                <v-skeleton-loader class="mb-2" type="text" max-width="200"></v-skeleton-loader>
                <v-skeleton-loader class="mb-2" type="text" max-width="200"></v-skeleton-loader>
            </v-sheet>
        </template>
        <!-- TODO: apply coupon -->
        <!-- <div></div> -->
    </div>
</template>


<script>
import { mapActions, mapGetters } from 'vuex';
// svg
import shield from './images/sheild.svg';
import exams from './images/exams.svg';
import secure from './images/secure.svg';

export default {
    name: 'whyUsDesktop',
    props: {
        document: {
            type: Object,
        }
    },
    components: {
        shield,
        exams,
        secure,
    },
    computed: {
        ...mapGetters(['accountUser', 'getBtnLoading']),

        showBlock() {
            if(this.isVideo) {
                if(this.isPurchased || this.docPrice <= 0) {
                    return false;
                }
                return true;
            } else if(!this.isVideo) {
                return true;
            }
            return false;
        },
        zeroPrice() {
            return (this.document && this.document.details && this.document.details.price === 0);
        },
        priceWithComma() {
            if(this.document && this.document.details) {
                return this.document.details.price.toLocaleString();
            }
            return null
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
                return false;
            }
            return true;
        },
        isLoading() {
            if(this.document && this.document.details && !this.getBtnLoading) {
                return false;
            }
            return true;
        },
        isVideo() {
            return this.document.documentType === 'Video';
        },
    },
    methods: {
        ...mapActions(['downloadDocument', 'updatePurchaseConfirmation']),
        
        openPurchaseDialog() {
            if(this.accountUser) {
                this.updatePurchaseConfirmation(true)
            } else {
                // this.$openDialog('login')
                this.$store.commit('setComponent', 'register')
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
}
</script>
<style lang="less">
    @import '../../../../../styles/mixin.less';

    .itemPage__side {
        padding: 10px;
        min-width: 292px;
        height: max-content;
        border-radius: 8px;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
        background-color: #ffffff;
        text-align: center;
        &__top {
            display: flex;
            justify-content: flex-end;
            align-items: baseline;
            color: #43425d;
            font-weight: bold;
            margin-bottom: 8px;
            
            &__price{
                font-size: 30px;
                margin-right: 4px;           
            }
            &__pts {
                font-weight: 600;
            }
        }
        &__credit {
            color: #43425d;
            font-size: 13px;
            margin-bottom: 10px;
            font-weight: 600;
        }     
        &__btn {
            padding: 0 50px;
            height: 44px;
            margin-bottom: 25px;
            text-transform: initial;
            font-weight: 600;
            font-size: 20px !important;

            div {
                margin-bottom: 2px;
            }
        }
        &__bottom {
            color: #43425d;
            padding-left: 20px;
            @media (max-width: @screen-xs) {
                flex-direction: column;
                max-width: 236px;
                margin: 0 auto;
            }

            &__cont{
            @media (max-width: @screen-xs) {
                padding-bottom: 14px;
                &:last-child{
                    padding-bottom: 0;
                }
            }
                display: flex;
                &__icon{
                    fill: red !important;
                    margin-right: 8px;
                }
                &__span{
                    font-size: 14px;
                    font-weight: normal;
                    font-stretch: normal;
                    font-style: normal;
                    line-height: 1.62;
                    letter-spacing: normal;
                }
            }
        }
        .skeletonWrap {
            .skeletonButton {
                .v-skeleton-loader__button {
                    width: 100%;
                    border-radius: 20px;
                }
            }
        }
    }
</style>