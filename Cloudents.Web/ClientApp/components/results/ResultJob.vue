<template v-once>
    <a class="d-block job-cell" target="_blank" :href="item.url">
       <span @click="$ga.event('Search_Results', 'job',`#${index+1}_${item.source}`)"> <div class="cell-title-wrap mb-1"><span class="cell-title">{{item.title}}</span></div>
        <div class="desc">
            <div class="desc-title-wrap"><span class="desc-title">Job description:</span></div>
            <p><span>{{item.responsibilities}}</span></p>
        </div>
        <v-layout row wrap class="properties">
            <v-flex class="property" sm4 xs6><company-icon class="mr-2"></company-icon><span class="company" :title="item.company">{{item.company?item.company:'N/A'}}</span></v-flex>
            <v-flex class="property" sm4 xs6><location-icon class="mr-2"></location-icon><span class="location" v-if="item.address">{{item.address}}</span></v-flex>
            <v-flex sm4 xs0></v-flex>
            <v-flex class="property mt-3" sm4 xs6><paid-icon class="mr-2"></paid-icon><span>{{item.compensationType}}</span></v-flex>
            <v-flex class="property mt-3" sm4 xs6><caldendar-icon class="mr-2"></caldendar-icon><span class="time">{{formatDate}}</span></v-flex>
            <v-flex sm4 xs0></v-flex>
        </v-layout></span>

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
            item: { type: Object, required: true },index:{Number}
        },

        computed:{
            formatDate(){
                let date_str=this.item.dateTime;
                let options = { year: 'numeric', month: 'short', day: 'numeric'},
                    formatted = (new Date(date_str)).toLocaleDateString('en-US', options),
                    date_parts = formatted.substring(0, formatted.indexOf(",")).split(" ").reverse().join(" ");

                return date_parts + formatted.substr(formatted.indexOf(",") + 1);
            }
        }
    }
</script>
<style src="./ResultJob.less" lang="less"></style>
