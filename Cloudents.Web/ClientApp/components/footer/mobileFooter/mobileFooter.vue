<template>
    <div>
        <v-bottom-nav
                :active.sync="activeBtn"
                :value="true"
                fixed
                app
                color="white"
        >
            <v-btn flat color="teal" value="feed" @click="closeActive()">
                <span class="mob-footer-title">Feed</span>
                <v-icon v-if="activeBtn !== 'feed'">sbf-icon-feed</v-icon>
                <v-icon v-else>sbf-icon-feed-selected</v-icon>

            </v-btn>
            <v-btn flat color="teal" value="earners">
                <span class="mob-footer-title">Earners</span>
                <v-icon v-if="activeBtn !== 'earners'">sbf-icon-earners</v-icon>
                <v-icon v-else>sbf-icon-earners-selected</v-icon>
            </v-btn>

            <v-btn flat color="teal"  value="promotions" @click="changemobileMarketingBoxState(true)">
                <span class="mob-footer-title">Promotions</span>
                <v-icon v-if="activeBtn !== 'promotions'">sbf-icon-promotions</v-icon>
                <v-icon v-else>sbf-icon-promotions-selected</v-icon>

            </v-btn>
        </v-bottom-nav>
    </div>
</template>
<script>
    import { mapActions } from 'vuex';
    export default {
        name: "mobileFooter",
        data() {
            return {
                activeBtn: 'feed',
                showNav: true
            }
        },

        computed: {},
        methods: {
            ...mapActions(['changemobileMarketingBoxState']),
            closeActive(){
            this.changemobileMarketingBoxState(false);
            }
        },
    }
</script>

<style lang="less">
    @import "../../../styles/mixin.less";

    .v-item-group.v-bottom-nav .v-btn--active .v-btn__content {
        color: @color-blue-new;
    }
    .v-item-group.v-bottom-nav .v-btn--active{
        /*padding-top: 0;*/
    }
    .mob-footer-title {
        opacity: 0.9;
        font-family: @fontOpenSans;
        /*font-size: 9px;*/
        font-weight: 600;
        letter-spacing: -0.1px;
        text-align: center;
    }

</style>