    <template>
    <div class="profileCoverActions text-center">
        <div class="mainTitle mb-2 white--text" v-t="'profile_title1'"></div>
        <div class="subTitle white--text" v-t="'profile_title2'"></div>

        <div class="mt-5">
            <v-btn class="btn white--text me-3" rounded depressed color="#ff6927" width="200" height="46" :block="isMobile">
                <chatIcon class="me-2" width="23" />
                <span v-t="'message_me'"></span>
            </v-btn>
            <v-btn class="btn white--text" rounded depressed color="#4c59ff" width="200" height="46" :block="isMobile">
                <calendarIcon class="me-2" width="23" />
                <span v-t="'book_lesson'"></span>
            </v-btn>
        </div>
    </div>
</template>

<script>
import chatIcon from './chat.svg'
import calendarIcon from './calendar.svg'

export default {
    name: 'profileCoverActions',
    components: {
        chatIcon,
        calendarIcon
    },
    data() {
        return {

        }
    },
    computed: {
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly
        }
    },
    methods: {

    }
}
</script>

<style lang="less">
.profileCoverActions {
    position: absolute;
    left: 0;
    right: 0;
    top: 350px;
    .mainTitle {
        font-size: 44px;
        font-weight: 600;
    }
    .subTitle {
        max-width: 450px;
        margin: 0 auto;
        font-size: 16px;
        font-weight: 600;
    }
    .btn {
        font-size: 16px;
        font-weight: 600;
    }
}
</style>