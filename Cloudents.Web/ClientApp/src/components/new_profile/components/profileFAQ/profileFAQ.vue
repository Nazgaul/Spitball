<template>
    <div class="profileFAQ mb-10">
        <div class="text-center">
            <div class="faqTitle" v-t="'faq'"></div>
            <div class="faqSubtitle mt-2 mb-8" v-t="'faq subtitle'"></div>

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
.profileFAQ {
    max-width: 960px;
    margin: 0 auto;
    color: #363637;
    .faqTitle {
        font-size: 36px;
        font-weight: 600;
    }
    .faqSubtitle {
        font-size: 18px;
    }
    .faqPanels {
        color: #363637;
        border: solid 1px #dddddd;
        border-radius: 6px;
        .faqHeader {
            font-size: 16px;
            .sbf-arrow-down {
                color: #ff6f30 !important;
            }
        }
    }
}
</style>