<template>
    <div class="profileSubscription pa-4 text-center">
        <div class="wrapper pa-4">
            <div class="mainTitle mb-1" v-t="'profile_subscribe_title'"></div>
            <div class="subTitle text-truncate" v-t="{path: 'profile_subscribe_subtitle', args: {0: firstName}}"></div>

            <div class="priceWrapper mt-8 mb-2">
                <span class="price">15$</span>
                <span v-t="'profile_subscribe_price_month'"></span>
            </div>

            <v-btn class="btnSubscribe white--text mb-2" height="42" width="180" @click="subscribeNow" depressed color="#41c4bc" rounded>
                <span v-t="'profile_subscribe_btn'"></span>
            </v-btn>

            <div class="noContract" v-t="'profile_subscribe_no_contract'"></div>
        </div>
    </div>
</template>

<script>
export default {
    name: 'profileSubscription',
    props: {
        id: {
            required: true
        }
    },
    computed: {
        firstName() {
            return this.$store.getters.accountUser?.firstName
        }
    },
    methods: {
        subscribeNow() {

        }
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

@greenColor: #41c4bc;

.profileSubscription {
    max-width: 960px;
    background: #fff;
    margin: 54px auto 0;
    border-radius: 8px;
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
    
    @media(max-width: @screen-xs) {
        margin: 8px auto;
        box-shadow: none;
        border-radius: 0;
    }

    .wrapper {
        border: solid 1.5px @greenColor;
        border-radius: 8px;
        .mainTitle {
            .responsive-property(font-size, 26px, null, 22px);                
            font-weight: bold;
            color: @greenColor;
            text-transform: uppercase;
        }

        .subTitle {
            color: @global-purple;
            .responsive-property(font-size, 20px, null, 16px);                
        }

        .priceWrapper {
            color: @global-purple;
            font-size: 18px;
            .price {
                font-size: 40px;
                font-weight: 600;
            }
        }
        .noContract {
            color: @global-purple;
            .responsive-property(font-size, 14px, null, 13px);
        }
    }
}
</style>