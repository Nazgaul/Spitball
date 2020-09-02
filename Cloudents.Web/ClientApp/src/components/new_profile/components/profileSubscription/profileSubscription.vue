<template>
    <div class="profileSubscription pa-4 text-center">
        <div class="subscriptionWrapper pa-4">
            <div class="mainTitle mb-1" v-t="'profile_subscribe_title'"></div>
            <div class="subTitle">{{$t('profile_subscribe_subtitle', [firstName])}}</div>

            <div class="priceWrapper mt-8 mb-2">
                <!-- TODO: Currency Change -->
                <span class="price">{{$price(subscriptionAmount.amount, subscriptionAmount.currency)}}</span>
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
        userId: {
            required: true
        }
    },
    computed: {
        subscriptionAmount() {
            //TODO - you are not doing it right
            return this.profileTutorSubscription
        },
        profileTutorSubscription() {
            return this.$store.getters?.getProfileTutorSubscription
        },
        firstName() {
            return this.$store.getters.getProfile?.user?.firstName
        },
        isLogged() {
            return this.$store.getters.getUserLoggedInStatus
        },
        isMyProfile() {
            return this.$store.getters.getIsMyProfile
        },
    },
    methods: {
        subscribeNow() {
            if(this.isMyProfile) {
                this.$emit('handleFollowMyProfile', this.$t('profile_subscribe_myself'))
                return
            }
            if(!this.isLogged) {
                sessionStorage.setItem('hash','#subscription');
                this.$store.commit('setComponent', 'register')
                return
            }
            this.$store.dispatch('subscribeToTutor', this.userId)
        }
    },
    mounted() {
        this.$loadScript("https://js.stripe.com/v3/?advancedFraudSignals=false")
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
    margin: 38px auto 0;
    border-radius: 8px;
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
    
    @media(max-width: @screen-xs) {
        margin: 8px auto;
        box-shadow: none;
        border-radius: 0;
    }

    .subscriptionWrapper {
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