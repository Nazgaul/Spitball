<template>
    <div>
        <v-bottom-nav
                height="48px"
                :active.sync="activeBtn"
                :value="true"
                fixed
                color="#f5f5f5"
                :app="$vuetify.breakpoint.xsOnly && getMobileFooterState()"
                class="notransition mob-footer-navigation"
        >
            <v-btn flat color="teal" value="feed" @click="changeActiveTab(tabs.feed)">
                <span class="mob-footer-title" v-language:inner>mobileFooter_action_feed</span>
                <v-icon class="mob-footer-icon" v-if="activeBtn !== tabs.feed">sbf-icon-feed</v-icon>
                <v-icon class="mob-footer-icon" v-else>sbf-icon-feed-selected</v-icon>

            </v-btn>
            <v-btn flat color="teal" value="earners"  @click="changeActiveTab(tabs.earners)">
                <span class="mob-footer-title" v-language:inner>mobileFooter_action_earners</span>
                <v-icon class="mob-footer-icon" v-if="activeBtn !== tabs.earners">sbf-icon-earners</v-icon>
                <v-icon class="mob-footer-icon" v-else>sbf-icon-earners-selected</v-icon>
            </v-btn>

            <v-btn flat color="teal" value="promotions"  @click="changeActiveTab(tabs.promotions)">
                <span class="mob-footer-title" v-language:inner>mobileFooter_action_promotion</span>
                <v-icon class="mob-footer-icon" v-if="activeBtn !== tabs.promotions">sbf-icon-promotions</v-icon>
                <v-icon class="mob-footer-icon" v-else>sbf-icon-promotions-selected</v-icon>
            </v-btn>
        </v-bottom-nav>
    </div>
</template>
<script>
    import { mapActions, mapGetters } from 'vuex';

    export default {
        name: "mobileFooter",
        props: {
            onStepChange: {
                type: Function,
                required: false
            },
        },
        data() {
            return {
                activeBtn: 'feed',
                showNav: true,
                tabs: this.getFooterEnumsState()
            }
        },

        computed: {

        },
        methods: {
            ...mapActions(['changemobileMarketingBoxState', 'changeFooterActiveTab']),
            ...mapGetters(["getFooterEnumsState", "getMobileFooterState"]),
            changeActiveTab(val){
                this.onStepChange();
                this.activeBtn = val;
                this.changeFooterActiveTab(val);
            }
        },
    }
</script>

<style lang="less">
    @import "../../../styles/mixin.less";
    .v-bottom-nav{
        &.mob-footer-navigation{
            box-shadow: none;
            border-top: 1px solid #d7d7d7!important;
        }
    }

    .mob-footer-icon{
        font-size: 20px;
    }
    .v-item-group.v-bottom-nav .v-btn--active .v-btn__content {
        font-size: 12px;
        color: @color-blue-new;
    }
    .v-item-group.v-bottom-nav .v-btn--active {
        padding-top: 8px;
    }

    .mob-footer-title {
        opacity: 0.9;
        font-family: @fontOpenSans;
        /*font-size: 9px;*/
        font-weight: 500;
        letter-spacing: -0.1px;
        text-align: center;
    }

</style>