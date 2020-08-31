<template>
    <v-row class="marketingActions pa-0 text-center">
        <!-- <v-col class="pa-0 mb-6 d-block d-sm-none justify-space-between" cols="12">
            <div class="text text-left" v-t="'marketing_title'"></div>
        </v-col> -->
        <template v-for="(data, index, i) in resource">
            <actionBox :key="index" :index="i" :currentCourseItem="currentCourseItem" :data="data" :len="resource.length" :isDashboard="$route.name === routeNames.Dashboard">
                <template #courseSelect v-if="i === 1">
                    <v-select
                        v-model="currentCourseItem"
                        :items="$store.getters.getCoursesItems"
                        hide-details
                        dense
                        item-text="name"
                        item-value="id"
                        menu-props="auto"
                        return-object
                        class="promoteCourseSelect me-3"
                        :label="$t('choose_course')"
                        outlined
                    ></v-select>
                </template>
            </actionBox>
        </template>
    </v-row>
</template>

<script>
import * as routeNames from '../../../../routes/routeNames'
import actionBox from './actionBox.vue';

export default {
    name: "marketingActions",
    components: {
        actionBox
    },
    props: {
        resource: {
            type: Object,
            required: true
        }
    },
    data() {
        return {
            routeNames,
            currentCourseItem: {},
            items: []
        }
    },
    created() {
        this.$store.dispatch('updateCoursesItems')
    }
}
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    @import '../../../../styles/colors.less';
    .marketingActions {
        width: 100%;
        margin: 0 auto;

        .text {
            color: @global-purple;
            font-weight: 600;
            .responsive-property(font-size, 20px, null, 16px);
        }

        .box {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: space-between;
            color: @global-purple;
            .text1 {
                font-size: 16px;
                font-weight: 600;
            }
            .text2 {
                font-size: 12px;
            }
            .marketingbtn {
                text-transform: initial;
                font-weight: 600;
                min-width: 120px;
                letter-spacing: normal;
                span {
                    margin-bottom: 2px;
                }
            }
            &:not(:last-child) {
                border-right: 1px solid #dddddd;

                @media (max-width: @screen-xs) {
                    border-bottom: 1px solid #dddddd;
                    border-right: none;
                }
            }
        }
        .promoteCourseSelect {
            border-radius: 20px;
            width: 190px;
            
            .v-select__slot {
                label {
                    font-size: 14px;
                    color: @global-purple;
                }
            }
        }
    }
</style>