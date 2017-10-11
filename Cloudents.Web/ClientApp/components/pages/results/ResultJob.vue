<template>
        <!--<a target="_blank" :href="item.url">
            <v-card>
                <v-card-title>title#:{{item.title}}</v-card-title>
                <v-card-text>
                    <span v-if="item.responsibilities">responsibilities:{{item.responsibilities}}</span>
                    <span v-if="item.city">city:{{item.city}}</span>
                    <span v-if="item.state">state:{{item.state}}</span>
                    <span v-if="item.compensationType">CompensationType:{{item.compensationType}}</span>
                    <span v-if="item.company">company:{{item.company}}</span>
                    <span v-if="item.dateTime">dateTime:{{item.dateTime}}</span>
                </v-card-text>
            </v-card>
        </a>-->
    <a target="_blank" :href="item.url" class="wrapper">
        <div class="container-box">
            <div class="box-main box-jobs-2">
                <div class="box-wrapper">
                    <div class="title">{{item.title}}</div>
                    <p>
                        <span class="descr">Job description:</span>
                        {{item.responsibilities}}
                    </p>
                    <v-layout class="box-options wrap" row>
                        <v-flex xs6 class=" mb-14">
                            <span class="img-option"><img src="img/svg/company-icon.svg" alt=""></span>
                            {{item.company}}
                        </v-flex>
                        <v-flex xs6 class="mb-14">
                            <span class="img-option"><img src="img/svg/location-icon.svg" alt=""></span>
                            {{item.city}}<span v-if="item.state">, {{item.state}}</span>
                        </v-flex>
                        <v-flex xs6>
                            <span class="img-option"><img src="img/svg/paid-icon.svg" alt=""></span>
                            {{item.compensationType}}
                        </v-flex>
                        <v-flex xs6>
                            <span class="img-option"><img src="img/svg/calendar-icon.svg" alt=""></span>
                            {{item.dateTime}}
                        </v-flex>
                    </v-layout>
                </div>
            </div>
        </div>
    </a>
</template>
<script>

    export default {
        props: { item: { type: Object, required: true } }
    }
</script>
<style src="./ResultJob.less" lang="less" scoped></style>
