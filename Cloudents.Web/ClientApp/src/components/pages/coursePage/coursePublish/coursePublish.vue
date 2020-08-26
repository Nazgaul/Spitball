<template>
    <div class="visibleSection pa-3 mb-4">
        <div class="d-flex align-center justify-space-between mb-3">
            <div class="courseStickyTitle" v-t="'visible'"></div>
            <v-switch
                v-model="courseVisible"
                :key="componentKeyRender"
                :disabled="!canCreateCourse"
                class="ma-0 pa-0"
                hide-details
            ></v-switch>
        </div>

        <div class="courseStickySubTitle" v-t="'course_visible'"></div>
    </div>
</template>

<script>
export default {
    name: 'coursePublish',
    data() {
        return {
            componentKeyRender: 0
        }
    },
    computed: {
        canCreateCourse() {
            let price = this.$store.getters.getFollowerPrice
            let canCreate = this.$store.getters.getIsCanCreateCourse
            let needPayment = this.$store.getters.getAccountNeedPayment
            if(global.country === 'IL') {
                return parseInt(price) === 0 || !needPayment
            }
            return canCreate
        },
        courseVisible: {
            get() {
                return this.$store.getters.getCourseVisible
            },
            set(val) {
                let needPayment = this.$store.getters.getAccountNeedPayment
                if(needPayment && global.country === 'IL') {
                    this.$store.commit('setShowCourse', false)
                    this.componentKeyRender += 1
                    this.showSnackbar = true
                    this.$emit('showError', {
                        text: this.$t('course_need_payment')
                    })
                    return
                }
                this.$store.commit('setShowCourse', val)
            }
        }
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.visibleSection {
    background: #fff;
    height: max-content;
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
    border-radius: 6px;
        .courseStickyTitle {
            font-size: 16px;
            color: @global-purple;
        }
        .courseStickySubTitle {
            color: @global-purple;
        }
    }
</style>