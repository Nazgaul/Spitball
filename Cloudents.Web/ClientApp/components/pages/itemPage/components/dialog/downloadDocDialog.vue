<template>

    <sb-dialog
        :showDialog="getDownloadDocDialog"
        :popUpType="'downloadDocDialog'"
        :activateOverlay="true"
        :isPersistent="true"
        :content-class="`downloadDocDialog`">
        <v-card class="downloadDocDialog">
            <v-progress-circular
                :rotate="-90"
                :size="100"
                :width="15"
                :value="value"
                color="primary">
                {{ value }}
            </v-progress-circular>
            <v-btn v-if="showBtn" color="success">text</v-btn>
        </v-card>
    </sb-dialog>

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import sbDialog from '../../../../wrappers/sb-dialog/sb-dialog.vue';

export default {
    name: 'downloadDocDialog',
    components: { sbDialog },
    data() {
        return {
            interval: {},
            value: 0,
            showBtn:false,
        }
    },
    beforeDestroy () {
      clearInterval(this.interval)
    },
    mounted () {
      this.interval = setInterval(() => {
        if (this.value >= 100) {
           this.showBtn = true;
           clearInterval(this.interval)
        }else{
            this.value += 10
        }
      }, 1000)
    },
    computed: {
        ...mapGetters(['getDownloadDocDialog']),
    },
    methods: {
        ...mapActions(['']),
    }
}
</script>

<style lang="less">
@import '../../../../../styles/mixin';

</style>