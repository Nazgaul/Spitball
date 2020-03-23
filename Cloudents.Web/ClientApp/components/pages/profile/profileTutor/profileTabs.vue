<template>
     <v-sheet class="profileTabs">
        <v-tabs
            background-color="#fff"
            color="#43425d"
            next-icon="mdi-arrow-right-bold-box-outline"
            prev-icon="mdi-arrow-left-bold-box-outline"
            show-arrows
            centered
        >
            <v-tabs-slider class="tabsSlider" color="blue"></v-tabs-slider>
            <v-tab
                v-for="(tab, index) in tabs"
                :key="index"
            >
            {{ tab.text }}
            </v-tab>
        </v-tabs>
    </v-sheet>
</template>

<script>
export default {
    name: 'profileTabs',
    data() {
        return {
            tabs: {
                home: {
                    text: this.$t('profileTutor_home'),
                    value: true,
                    action: this.test
                },
                about: {
                    text: this.$t('profileTutor_about'),
                    value: false,
                    action: this.test
                },
                review: {
                    text: this.$t('profileTutor_review'),
                    value: false,
                    action: this.test
                },
                study: {
                    text: this.$t('profileTutor_study'),
                    value: false,
                    action: this.test
                },
                questions: {
                    text: this.$t('profileTutor_questions'),
                    value: false,
                    action: this.test
                },
                calendar: {
                    text: this.$t('profileTutor_calendar'),
                    value: false,
                    action: this.test
                },
                faq: {
                    text: this.$t('profileTutor_faq'),
                    value: false,
                    action: this.test
                },
            }
        }
    }
}
</script>

<style lang="less">
    .profileTabs {


        .v-tabs-slider-wrapper {
            top: 0;
        }
    }
</style>