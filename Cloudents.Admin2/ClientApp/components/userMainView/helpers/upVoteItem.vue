<template>
    <div>
        <v-layout align-start column v-for="(upItem, index) in upvotedItems" :key="`upvote`+index" class="mb-3 px-2 py-2 elevation-2">
            <v-flex xs12 md12 sm12 class="py-1">
                <span class="label subheading text-xs-left text-md-left mr-3">Document ID:</span>
                <span class="font-weight-medium">  {{upItem.created}}</span>

            </v-flex>
            <v-flex class="py-1" xs12 md12 sm12><span class="label subheading text-xs-left text-md-left mr-3">Document name:</span>
                <span class="font-weight-medium">{{upItem.itemText}}</span>

            </v-flex>
            <v-flex class="py-1" xs12 md12 sm12><span class="label subheading mr-3">University:</span>
                <span class="font-weight-medium">{{upItem.itemType}}</span>
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
    export default {
        name: "upVoteItem",
        data() {
            return {}
        },
        props: {
            upvotedItems: {},
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