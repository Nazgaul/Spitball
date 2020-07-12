<template>
    <v-snackbar
        top
        :timeout="5000"
        :value="true"
    >
    <div class="text-center">{{params.text}}</div>
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