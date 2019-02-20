<template>
    <div>
        <v-layout align-start column v-for="(downItem, index) in downvotedItems" :key="`down`+index" class="mb-3 px-2 py-2 elevation-2">
            <v-flex xs12 md12 sm12 class="py-1">
                <span class="label subheading text-xs-left text-md-left mr-3">Up Vote Date:</span>
                <span class="font-weight-medium">{{downItem.date | dateFromISO}}</span>

            </v-flex>
            <v-flex class="py-1" xs12 md12 sm12><span class="label subheading text-xs-left text-md-left mr-3">Text:</span>
                <span class="font-weight-medium">{{downItem.text}}</span>

            </v-flex>
            <v-flex class="py-1" xs12 md12 sm12><span class="label subheading mr-3">Item Type:</span>
                <span class="font-weight-medium">{{downItem.type}}</span>
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
    export default {
        name: "downVoteItem",
        data() {
            return {}
        },
        props: {
            downvotedItems: {},
            filterVal: {
                type: String,
                required: false
            },

        },
        methods: {}
    }
</script>

<style scoped>

</style>