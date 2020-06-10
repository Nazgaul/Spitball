<template>
    <v-snackbar
        absolute
        top
        :timeout="5000"
        :value="true"
    >
    <span>{{params.text}}</span>
    </v-snackbar>
</template>


<script>

export default {
    name: 'buyPointsTransaction',
    props: {
        params: {
            type:Object,
            required:true
        }
    },
}
</script>