<template>
    <div>
        <a target="_blank" :href="item.url" v-for="(item,i) in items" :key="i">
                <v-card-title>title#{{i}}:{{item.title}}</v-card-title>
                <v-card-text>
                    <span v-if="item.responsibilities">responsibilities:{{item.responsibilities}}</span>
                    <span v-if="item.city">city:{{item.city}}</span>
                    <span v-if="item.state">state:{{item.state}}</span>
                    <span v-if="item.compensationType">CompensationType:{{item.compensationType}}</span>
                    <span v-if="item.company">company:{{item.company}}</span>
                    <span v-if="item.dateTime">dateTime:{{item.dateTime}}</span>
                </v-card-text>
        </a>
    </div>
</template>
<script>
    import { itemsList } from './mixins'

    export default {
        mixins:[itemsList]
    }
</script>