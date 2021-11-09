<template>
  <div id="invmain">
    <appbar />
    <v-container>
      <v-card>
        <v-card-title>Search Invoice Number</v-card-title>
        <v-card-text>
          <v-form>
            <v-row>
              <v-col cols="12" sm="6" md="6">
                <v-text-field
                  v-model="formSerch.invoiceNo"
                  label="Invoice Number"
                  outlined
                  dense
                ></v-text-field
              ></v-col>
              <v-col cols="12" sm="6" md="6">
                <v-combobox
                  v-model="formSerch.lineOperator"
                  dense
                  outlined
                  label="Line Operator"
                  :items="items"
                ></v-combobox>
              </v-col>
              <!-- <v-col cols="12" sm="2" md="2">
                <v-checkbox
                  v-model="manualInv"
                  label="Manual Invoice"
                  color="red"
                  hide-details
                ></v-checkbox>
              </v-col> -->
              <v-col cols="12">
                <v-card-actions>
                  <v-row>
                    <v-col cols="12" sm="6" md="6">
                      <v-btn color="#fb8c00" dark @click="validateSearch"
                        >Search Invoice</v-btn
                      >
                      <v-btn color="#632424" dark @click="clearpage"
                        >Clear Data</v-btn
                      ></v-col
                    >
                  </v-row>
                </v-card-actions>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
      </v-card>
    </v-container>
    <v-container>
      <v-card>
        <v-card-title>
          Result : Invoice number {{ formResult.resultInv }}
          <v-spacer></v-spacer>
          <v-text-field
            v-model="search"
            append-icon="mdi-magnify"
            label="Search"
            single-line
            hide-details
          ></v-text-field>
        </v-card-title>
        <v-card-text
          ><template>
            <v-data-table
              :headers="headers"
              :items="desserts"
              :items-per-page="10"
              :search="search"
              :loading="loading"
            >
            </v-data-table>
          </template>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn
            color="#4d0026"
            dark
            @click="validateCreate"
            :disabled="isBTNValid"
            >Create EDI</v-btn
          >
          <v-btn :disabled="isBTNValid" @click="onExport" dark color="#1f7043"
            >Excel</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-container>

    <v-dialog v-model="dialog" width="500" persistent>
      <v-card>
        <v-card-title class="text-h5 blue lighten-2">
          Information
        </v-card-title>

        <v-card-text v-html="msg" class="setdialog"></v-card-text>

        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn
            color="primary"
            @click="
              dialog = false
              msg = ''
            "
            >OK</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script>
import appbar from '@/components/appbar/appbar'
import XLSX from 'xlsx' // import xlsx

export default {
  name: 'invmain',
  components: { appbar },

  data() {
    return {
      dialog: false,
      msg: '',
      loading: false,
      isBTNValid: true,

      // manualInv: false,

      formSerch: {
        invoiceNo: this.$store.getters.getformSearch.invoiceNo,
        lineOperator: this.$store.getters.getformSearch.lineOperator,
      },
      formResult: {
        resultInv: '',
        data: '',
      },
      items: [],
      search: '',
      excelHeader: [
        [
          'No',
          'Invoice No',
          'Vessel Visit',
          'Vessel Name',
          'Container No',
          'Size',
          'Type',
          'Ladend Status',
          'Shiiping Status',
          'Servide Type',
          'Sevice Description',
        ],
      ],
      headers: [
        {
          text: 'No',
          value: 'NO',
          sortable: false,
        },
        {
          text: 'Invoice No',
          align: 'start',
          value: 'INVOICE_AN',
        },
        {
          text: 'VSI VISIT',
          value: 'VOYAGE_TERMINAL',
        },
        {
          text: 'VSI NAME',
          value: 'VESSEL_NM',
        },
        {
          text: 'CONTAINER',
          value: 'EQUIPMENT_AN',
        },
        {
          text: 'SIZE',
          value: 'EQUIPMENT_LENGTH_QT',
        },
        {
          text: 'TYPE',
          value: 'EQUIPMENT_TYPE_AN',
        },
        {
          text: 'F/E',
          value: 'EQUIPMENT_LADEN_STATUS',
        },
        {
          text: 'SS',
          value: 'EQUIPMENT_STATUS',
        },
        {
          text: 'SERVICE TYPE',
          value: 'SERVICE_TYPE_DS',
        },
        {
          text: 'CHARGE LINE',
          value: 'CHARGE_LINE_DS',
        },
      ],
      desserts: [this.$store.getters.getresultTB.resultTB],
    }
  },

  mounted: function () {
    this.LineOPRCode()
  },

  methods: {
    LineOPRCode() {
      this.$axios.post('/invedi.asmx/linerProfile').then((resp) => {
        let result = JSON.parse(this.converToJsonData(resp.data))
        // console.log(result)

        result.forEach((element) => {
          // console.log(element.LINE)
          this.items.push(element.LINE)
        })
      })
    },
    converToJsonData(dataXML) {
      let status = null
      const parser = new DOMParser()
      const dom = parser.parseFromString(dataXML, 'application/xml')

      if (
        dom.getElementsByTagName('string')[0].childNodes[0].nodeValue != null
      ) {
        status = dom.getElementsByTagName('string')[0].childNodes[0].nodeValue
        console.log(
          dom.getElementsByTagName('string')[0].childNodes[0].nodeValue
        )
      }

      return status
    },
    async validateSearch() {
      if (this.formSerch.invoiceNo == '' || this.formSerch.lineOperator == '') {
        // alert('Enter Invoice Number and Select Line Operator before search')
        this.msg = 'Enter Invoice Number and Select Line Operator before search'
        this.dialog = true
      }
      //  else if (this.manualInv) {
      //   console.log('Manual invoice')
      // }
      else {
        this.desserts.splice(0)
        this.loading = true
        this.formResult.resultInv = this.formSerch.invoiceNo

        var body = new URLSearchParams()
        body.append('invoice', this.formSerch.invoiceNo)
        body.append('Lineoper', this.formSerch.lineOperator)

        await this.$axios
          .post('/invedi.asmx/searchInv', body, {
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
          })
          .then((resp) => {
            let result = JSON.parse(this.converToJsonData(resp.data))
            // console.log(result)
            if (result.length > 0) {
              result.forEach((element) => {
                // console.log(element.LINE)

                this.desserts.push(element)

                console.log(element)
              })

              this.isBTNValid = false
            } else {
              this.msg = `Not found data for this invoice number : <b> ${this.formSerch.invoiceNo} </b>`
              this.dialog = true
            }
          })
          .catch((error) => {
            console.log(error)
            this.msg = 'Web API not response <br /> Please contact IT for help'
            this.dialog = true
          })
        this.loading = false
      }
    },
    validateCreate() {
      if (this.formSerch.invoiceNo == '' || this.formSerch.lineOperator == '') {
        this.msg =
          'Enter Invoice Number and Select Line Operator before Create EDI file.'
        this.dialog = true
      } else {
        this.$store.dispatch('setformSearch', this.formSerch)
        this.$store.dispatch('setresultTB', this.desserts)
        this.$router.push(`/${this.$store.getters.getformSearch.lineOperator}`)

        // this.$router.push('/generate')
      }
    },
    clearpage() {
      this.formSerch.invoiceNo = ''
      this.formSerch.lineOperator = ''
      this.formResult.resultInv = ''
      this.formResult.data = ''
      this.desserts.splice(0)
      this.isBTNValid = true
      this.loading = false
    },
    onExport() {
      if (this.desserts.length > 0) {
        let dataWS = XLSX.utils.json_to_sheet(this.desserts, {
          skipHeader: true,
        })
        XLSX.utils.sheet_add_aoa(dataWS, this.excelHeader)
        const wb = XLSX.utils.book_new()
        XLSX.utils.book_append_sheet(wb, dataWS)
        XLSX.writeFile(wb, `Inv ${this.formSerch.invoiceNo}.xlsx`)
      } else {
        this.msg = 'No data available'
        this.dialog = true
      }
    },
  },
}
</script>

<style>
.v-btn {
  height: 40px !important;
  padding-left: 5px;
  margin-right: 5px;
  padding-top: 5px;
  margin-top: 5px;
  width: 160.05px;
}
.v-input__control {
  height: 46px;
}
.setdialog {
  margin-top: 10px;
}

.v-input--selection-controls {
  padding-top: 10px;
}

/*
.text-start span {
  color: white;
}
.theme--light.v-data-table
  .v-data-table-header
  th.sortable.active
  .v-data-table-header__icon {
  color: rgba(60, 255, 0, 0.87);
} */
</style>
