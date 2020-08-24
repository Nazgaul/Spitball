<template>
    <v-snackbar
        top
        :timeout="5000"
        :value="true"
        @input="onCloseToaster"
    >
    <div class="text-center">{{params.text}}</div>
    </v-snackbar>
</template>


<script>

export default {
    name: 'PURCHASE_TRANSACTION',
    props: {
        params: {
            type:Object,
            required:true
        }
    },
    methods: {
        onCloseToaster() {
            this.$store.commit('removeComponent',this.params.name)
        }
    },
}
</script>