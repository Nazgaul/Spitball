<template>
    <div class="itemPage__side" v-if="showBlock && !isLoading">
        <template v-if="!zeroPrice && !isPurchased">
            <div class="itemPage__side__top">
                <h1 class="itemPage__side__top__h1">{{priceWithComma}}</h1>
                <span class="itemPage__side__top__pts" v-language:inner="'documentPage_points'"></span>
            </div>
            <h5 class="itemPage__side__h5" v-language:inner="'documentPage_credit_uploader'"></h5>
        </template>
        <v-btn 
            class="itemPage__side__btn white--text"
            depressed
            block
            round
            :loading="isLoading"
            @click="openPurchaseDialog"
            v-if="!isPurchased || isVideo"
            color="#4c59ff">
                <span v-language:inner="'documentPage_unlock_btn'"></span>
        </v-btn>
        <v-btn
            v-else
            tag="a"
            :href="`${$route.path}/download`"
            target="_blank"
            :loading="isLoading"
            class="itemPage__side__btn white--text"
            depressed block round @click="downloadDoc" color="#4c59ff">
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
    data() {
        return {
            btnLoader: true,
        }
    },
    computed: {
        ...mapGetters(['accountUser', 'getBtnLoading']),

        showBlock() {
            if(this.isVideo && !this.isShowPurchased) {
                return false;
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
                return false;
            }
            return true;
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

    .itemPage__side {
        padding: 10px;
        position: sticky;
        top: 80px;
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
            
            &__h1 {
                font-size: 30px;
                margin-right: 4px;
            }
            &__pts {
                font-weight: 600;
            }
        }
        &__h5 {
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
            font-size: 20px;

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
    }
</style>