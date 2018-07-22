import VPaperText from '../components/paper/VPaperText.vue'
import VPaperNumber from '../components/paper/VPaperNumber.vue'
import VPaperNumberInt from '../components/paper/VPaperNumberInt.vue'
import VPaperCheckbox from '../components/paper/VPaperCheckbox.vue'
import VPaperHidden from '../components/paper/VPaperHidden.vue'
import VPaperDate from '../components/paper/VPaperDate.vue'
import VPaperTime from '../components/paper/VPaperTime.vue'
import VPaperSelect from '../components/paper/VPaperSelect.vue'
import VPaperSwitch from '../components/paper/VPaperSwitch.vue'
import VPaperDatetime from '../components/paper/VPaperDatetime.vue'
import VPaperCurrency from '../components/paper/VPaperCurrency.vue'
import VPaperUrl from '../components/paper/VPaperUrl.vue'
import VPaperLabel from '../components/paper/VPaperLabel.vue'
import VPaperDateRange from '../components/paper/VPaperDateRange.vue'
export default {
  data: () => ({
    Type: {
      HIDDEN: 'hidden',
      TEXT: 'text',
      SEARCH: 'search',
      TEL: 'tel',
      URL: 'url',
      EMAIL: 'email',
      PASSWORD: 'password',
      DATETIME: 'datetime',
      DATE: 'date',
      MONTH: 'month',
      WEEK: 'week',
      TIME: 'time',
      NUMBER: 'number',
      RANGE: 'range',
      COLOR: 'color',
      CHECKBOX: 'checkbox',
      RADIO: 'radio',
      FILE: 'file'
    },
    DataType: {
      BIT: 'bit',
      BOOLEAN: 'boolean',
      BOOL: 'bool',
      NUMBER: 'numbr',
      INT: 'int',
      LONG: 'long',
      DECIMAL: 'decimal',
      DOUBLE: 'double',
      FLOAT: 'float',
      CURRENCY: 'currency',
      STRING: 'string'
    }
  }),
  components: {
    VPaperText,
    VPaperNumber,
    VPaperNumberInt,
    VPaperCheckbox,
    VPaperHidden,
    VPaperDate,
    VPaperTime,
    VPaperSelect,
    VPaperSwitch,
    VPaperDatetime,
    VPaperCurrency,
    VPaperUrl,
    VPaperDateRange,
    VPaperLabel
  },
  methods: {
    $_formsMixin_dynamicComponent (field) {
      switch (field.type) {
        case this.Type.HIDDEN:
          return 'VPaperHidden'
        case this.Type.DATE:
          return 'VPaperDate'
        case this.Type.TIME:
          return 'VPaperTime'
        case this.Type.URL:
          return 'VPaperUrl'
        case this.Type.TEXT:
          return this._getSubDataType(field.dataType)
        case this.Type.NUMBER:
          return this._getSubDataType(field.dataType)
        case this.Type.CHECKBOX:
          return 'VPaperSelect'
        default:
          return 'VPaperText'
      }
    },

    $_formsMixin_makeParams (actionName) {
      var params = {}
      var formName = 'form-' + actionName
      var form = this.$refs[formName]
      form.inputs.forEach((field) => {
        var fieldValue = field.$attrs.value
        if (fieldValue && fieldValue.length > 0) {
          var param = field.$attrs.name
          var value = field.$attrs.value
          params[param] = value
        }
      })
      return params
    },

    $_formsMixin_clear (actionName) {
      var formName = 'form-' + actionName
      var form = this.$refs[formName]
      form.reset()
    },

    $_formsMixin_haveInputs (action) {
      var input = action.fields.find(field =>
        field.type !== this.Type.TEXT && field.dataType !== this.DataType.STRING
      )
      return input !== undefined
    },

    _getSubDataType (dataType) {
      switch (dataType) {
        case this.DataType.BIT:
        case this.DataType.BOOL:
        case this.DataType.BOOLEAN:
          return 'VPaperSwitch'
        case this.DataType.NUMBER:
        case this.DataType.INT:
        case this.DataType.LONG:
          return 'VPaperNumber'
        case this.DataType.DECIMAL:
        case this.DataType.DOUBLE:
        case this.DataType.FLOAT:
          return 'VPaperNumber'
        case this.DataType.CURRENCY:
          return 'VPaperCurrency'
        case this.DataType.STRING:
          return 'VPaperLabel'
        default:
          return 'VPaperText'
      }
    }
  }
}
