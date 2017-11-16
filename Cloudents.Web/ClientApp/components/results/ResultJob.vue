<template v-once>
    <a class="d-block pa-2 job-cell" target="_blank" :href="item.url">
        <div class="cell-title mb-1">{{item.title}}</div>
        <div class="desc">
            <div class="mb-2">Job description:</div>
            <p>{{item.responsibilities}}</p>
        </div>
        <v-container class="pa-0">
            <v-layout row wrap>
                <v-flex xs6><company-icon class="mr-2"></company-icon>{{item.company}}</v-flex>
                <v-flex xs6><location-icon class="mr-2"></location-icon><span class="location" v-if="item.city">{{item.city}}, {{item.state}}</span></v-flex>
                <v-flex xs6><paid-icon class="mr-2"></paid-icon>{{item.compensationType}}</v-flex>
                <v-flex xs6><caldendar-icon class="mr-2"></caldendar-icon> {{item.dateTime | moment("D MMM YYYY")}}</v-flex>
            </v-layout>
        </v-container>

    </a>
</template>
<script>
    import caldendarIcon from "./svg/calendar-icon.svg";
    import companyIcon from "./svg/company-icon.svg";
    import locationIcon from "./svg/location-icon.svg";
    import paidIcon from "./svg/paid-icon.svg";
    export default {
        components: { caldendarIcon, companyIcon, locationIcon, paidIcon },
        props: {
            item: { type: Object, required: true }
        }
    }
</script>
<style src="./ResultJob.less" lang="less"></style>
