<template>
        <a target="_blank" :href="item.url">
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
        </a>
</template>
<script>

    export default {
        props: { item: { type: Object, required: true } }
    }
</script>