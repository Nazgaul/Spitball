<template>
    <div class="profileFAQ mb-10">
        <div class="text-sm-center ps-6">
            <div class="faqTitle mb-2" v-t="'faq'"></div>
            <div class="faqSubtitle mb-8 d-none d-sm-block" v-t="'faq subtitle'"></div>
        </div>

        <v-expansion-panels class="faqPanels" accordion>
            <v-expansion-panel
            v-for="(item,i) in faqItems"
            :key="i"
            >
                <v-expansion-panel-header class="faqHeader" :expand-icon="'sbf-arrow-down'">{{item.title}}</v-expansion-panel-header>
                <v-expansion-panel-content>
                    {{item.answer}}
                </v-expansion-panel-content>
            </v-expansion-panel>
    </v-expansion-panels>
    </div>
</template>

<script>
export default {
    name: 'profileFAQ',
    computed: {
        faqItems() {
            return this.$store.getters.getProfileFaq
        }
    },
    created() {
        this.$store.dispatch('updateProfileFaq')
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin';

.profileFAQ {
    max-width: 960px;
    margin: 0 auto;
    color: #363637;
    .faqTitle {
        .responsive-property(font-size, 36px, null, 24px);
        font-weight: 600;
    }
    .faqSubtitle {
        font-size: 18px;
    }
    .faqPanels {
        color: #363637;
        border: solid 1px #dddddd;
        border-radius: 6px;

        @media (max-width: @screen-xs) {
            border-radius: 0
        }
        .v-expansion-panel::before {
            box-shadow: none;
        }
        .faqHeader {
            font-size: 16px;
            .sbf-arrow-down {
                color: #ff6f30 !important;
            }
        }
    }
}
</style>