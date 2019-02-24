<template>
    <div>
        <v-layout  align-start column v-for="(flagged, index) in flaggedItems" :key="`flagg`+index" class="mb-3 px-2 py-2 elevation-2">
            <v-flex xs12 md12 sm12 class="py-1">
                <span class="label subheading text-xs-left text-md-left mr-3">Action Date:</span>
                <span class="font-weight-medium">  {{flagged.date | dateFromISO}}</span>

            </v-flex>
            <v-flex class="py-1" xs12 md12 sm12><span class="label subheading text-xs-left text-md-left mr-3">Item Text:</span>
                <span class="font-weight-medium">{{flagged.text}}</span>

            </v-flex>
            <v-flex class="py-1" xs12 md12 sm12><span class="label subheading mr-3">Flag Reason:</span>
                <span class="font-weight-medium">{{flagged.flagReason}}</span>
            </v-flex>
            <v-flex class="py-1" xs12 md12 sm12><span class="label subheading mr-3">Item Type:</span>
                <span class="font-weight-medium">{{flagged.type}}</span>
            </v-flex>
            <v-flex class="py-1" xs12 md12 sm12><span class="label subheading mr-3">Current State:</span>
                <span class="font-weight-medium">{{flagged.state}}</span>
            </v-flex>
            <v-flex class="py-1" xs12 md12 sm12><span class="label subheading mr-3">Votes:</span>
                <span class="font-weight-medium">{{flagged.voteCount}}</span>
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
    export default {
        name: "flaggedItem",
        data() {
            return {}
        },
        props: {
            flaggedItems: {},
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