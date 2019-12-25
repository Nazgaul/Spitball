<template>

    <sb-dialog
        :showDialog="showPurchaseConfirmation"
        :popUpType="'purchaseConfirmation'"
        :activateOverlay="true"
        :isPersistent="true"
        :content-class="`confirmation-purchase-dialog`"
    >
        <v-card class="confirm-purchase-card">
        <v-card-title class="confirm-headline">
            <span v-html="$Ph('preview_about_to_buy', [docPrice, uploaderName])"></span>
        </v-card-title>
        <v-card-actions class="card-actions">
            <div class="doc-details">
            <div class="doc-type" v-if="!isVideo">
                <v-icon class="doc-type-icon">sbf-document-note</v-icon>
            </div>
            <div class="doc-title">
                <div class="text-truncate">{{docTitle}}</div>
            </div>
            </div>
            <div class="purchase-actions">
            <v-btn flat class="cancel" @click.native="updatePurchaseConfirmation(false)">
                <span v-language:inner>preview_cancel</span>
            </v-btn>
            <v-btn round class="submit-purchase" @click.native="unlockDocument">
                <span class="hidden-xs-only" v-language:inner>preview_buy_btn</span>
                <span
                class="hidden-sm-and-up text-uppercase"
                v-language:inner
                >preview_itemActions_buy</span>
            </v-btn>
            </div>
        </v-card-actions>
        </v-card>
    </sb-dialog>

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import sbDialog from '../../../../wrappers/sb-dialog/sb-dialog.vue';

export default {
    name: 'unlockDialog',
    components: { sbDialog },
    props: {
        document: {
            type: Object,
            required: true
        }
    },
    data() {
        return {

        }
    },
    computed: {
        ...mapGetters(['accountUser', 'getPurchaseConfirmation']),

        showPurchaseConfirmation() {
            return this.getPurchaseConfirmation;
        },
        uploaderName() {
            if (this.document.details && this.document.details.user.name) {
                return this.document.details.user.name;
            }
            return null
        },
        docPrice() {
            if (this.document.details && this.document.details.price >= 0) {
                return this.document.details.price.toFixed(2);
            }
            return null
        },
        isVideo(){      
            return this.document.documentType === 'Video';
        },
        docTitle() {
            if (this.document.details && this.document.details.feedItem) {
                return this.document.details.feedItem.title;
            }
            return null
        },
    },
    methods: {
        ...mapActions(['updatePurchaseConfirmation', 'purchaseDocument']),

        unlockDocument() {
            let item = {
                id: this.document.details.id,
                price: this.document.details.price
            };
            this.purchaseDocument(item);
            this.updatePurchaseConfirmation(false);
        },
    }
}
</script>

<style lang="less">
    @import '../../../../../styles/mixin';

.confirmation-purchase-dialog {
  max-width: 544px !important;
  @media (max-width: @screen-xs) {
    max-width: 338px !important;
  }
  .confirm-purchase-card {
    justify-content: center;
    align-items: flex-start;
    border-radius: 4px;
    box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.33);
    overflow: hidden;
    @media (max-width: @screen-xs) {
      align-items: center;
    }
    .confirm-headline {
      font-size: 18px;
      line-height: 1.56;
      color: @color-blue-new;
      display: flex;
      flex-grow: 1;
      align-items: center;
      justify-content: center;
      width: 100%;
      padding: 32px 16px 24px 16px;
      @media (max-width: @screen-xs) {
        text-align: center;
        padding-top: 48px;
        padding-bottom: 32px;
      }
    }

    .card-actions {
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
      width: 100%;
      background: #f7f7f7;
      padding: 24px 16px 16px 16px;
      @media (max-width: @screen-xs) {
        padding: 32px 16px 16px 16px;
      }
      .doc-details {
        display: flex;
        // flex-direction: column;
        justify-content: center;
        align-items: center;

        .doc-type {
          display: flex;
          flex-direction: row;
          align-items: center;
          padding: 0 0 12px;
          .doc-type-icon {
            margin-right: 8px;
            color: @color-blue-new;
            font-size: 24px;
          }
          .doc-type-text {
            color: @color-blue-new;
            font-size: 13px;
          }
        }
        .doc-title {
          color: @textColor;
          text-align: center;
          font-size: 16px;
          font-weight: 600;
          // max-width: 400px;
          div {
            max-width: 200px;
          }
        }
      }
      .purchase-actions {
        display: flex;
        flex-direction: row;
        padding-top: 28px;
        @media (max-width: @screen-xs) {
          padding-top: 48px;
        }
        button {
          height: unset;
          background-color: transparent;
          font-size: 16px;
          text-transform: capitalize;
          &.submit-purchase {
            .sb-rounded-medium-btn();
            font-size: 16px;
          }
          &.cancel {
            font-size: 14px;
            color: fade(@color-black, 72%);
          }
        }
      }
    }
  }
}
</style>